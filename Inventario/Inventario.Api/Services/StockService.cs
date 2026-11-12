using System.Data;
using Inventario.Api.Contracts;
using Inventario.Api.Data;
using Inventario.Api.Infrastructure;
using Inventario.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Api.Services;

public class StockService(InventarioContext db, CatalogoClient catalogo)
{
    // Todos los cambios de una pareja producto/bodega toman el mismo bloqueo SQL.
    // UPDLOCK evita que dos solicitudes lean simultáneamente el mismo stock libre.
    private Task<Existencia?> Bloquear(int producto, int bodega) => db.Existencias
        .FromSqlInterpolated($"SELECT * FROM Existencia WITH (UPDLOCK, HOLDLOCK) WHERE IdProducto={producto} AND IdBodega={bodega}")
        .SingleOrDefaultAsync();

    private async Task<int> Reservado(int producto, int bodega) => await db.Reservas
        .Where(x => x.IdProducto == producto && x.IdBodega == bodega && x.Estado == "Activa")
        .SumAsync(x => (int?)x.Cantidad) ?? 0;

    public async Task<MovimientoInventario> Mover(MovimientoSolicitud dto)
    {
        await catalogo.ValidarProducto(dto.IdProducto);
        if (!await db.Bodegas.AnyAsync(x => x.IdBodega == dto.IdBodega)) throw new ApiException(400, "La bodega no existe.");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var e = await Bloquear(dto.IdProducto, dto.IdBodega);
        if (e is null)
        {
            if (dto.Tipo == "Salida") throw new ApiException(409, "No hay existencias para realizar la salida.");
            e = new Existencia { IdProducto = dto.IdProducto, IdBodega = dto.IdBodega };
            db.Existencias.Add(e);
        }
        var reservado = await Reservado(dto.IdProducto, dto.IdBodega);
        if (dto.Tipo == "Salida" && e.Stock - reservado < dto.Cantidad)
            throw new ApiException(409, "Stock disponible insuficiente; se respetan las reservas activas.");
        if (dto.Tipo == "Entrada" && e.Stock > int.MaxValue - dto.Cantidad)
            throw new ApiException(400, "La entrada supera el límite de stock.");
        e.Stock += dto.Tipo == "Entrada" ? dto.Cantidad : -dto.Cantidad;
        var movimiento = new MovimientoInventario
        {
            IdProducto = dto.IdProducto, IdBodega = dto.IdBodega, Tipo = dto.Tipo, Cantidad = dto.Cantidad
        };
        db.Movimientos.Add(movimiento);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return movimiento;
    }

    public async Task<ReservaStock> Reservar(ReservaSolicitud dto)
    {
        await catalogo.ValidarProducto(dto.IdProducto);
        if (!await db.Bodegas.AnyAsync(x => x.IdBodega == dto.IdBodega)) throw new ApiException(400, "La bodega no existe.");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var e = await Bloquear(dto.IdProducto, dto.IdBodega);
        var reservado = await Reservado(dto.IdProducto, dto.IdBodega);
        if (e is null || e.Stock - reservado < dto.Cantidad) throw new ApiException(409, "Stock disponible insuficiente para reservar.");
        var r = new ReservaStock { IdProducto = dto.IdProducto, IdBodega = dto.IdBodega, Cantidad = dto.Cantidad };
        db.Reservas.Add(r);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return r;
    }

    public async Task<ReservaStock> CerrarReserva(int id, bool consumir)
    {
        // Se consulta sin seguimiento para volver a leer después de obtener el bloqueo.
        var referencia = await db.Reservas.AsNoTracking().SingleOrDefaultAsync(x => x.IdReserva == id)
            ?? throw new ApiException(404, "La reserva no existe.");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var e = await Bloquear(referencia.IdProducto, referencia.IdBodega);
        var r = await db.Reservas.SingleAsync(x => x.IdReserva == id);
        if (r.Estado != "Activa") throw new ApiException(409, "La reserva ya fue cerrada.");
        if (consumir)
        {
            if (e is null || e.Stock < r.Cantidad) throw new ApiException(409, "Stock físico insuficiente.");
            e.Stock -= r.Cantidad;
            db.Movimientos.Add(new MovimientoInventario
            {
                IdProducto = r.IdProducto, IdBodega = r.IdBodega, Tipo = "Salida", Cantidad = r.Cantidad
            });
        }
        r.Estado = consumir ? "Consumida" : "Cancelada";
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return r;
    }
}
