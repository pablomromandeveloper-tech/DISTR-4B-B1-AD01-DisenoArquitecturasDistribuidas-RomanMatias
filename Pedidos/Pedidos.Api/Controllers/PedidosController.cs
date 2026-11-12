using System.Data;
using Pedidos.Api.Contracts;
using Pedidos.Api.Data;
using Pedidos.Api.Models;
using Pedidos.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Pedidos.Api.Controllers;

[ApiController, Route("api/pedidos")]
public class PedidosController(PedidosContext db, PedidoService servicio) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await db.Pedidos.AsNoTracking().Include(x => x.Detalles).OrderBy(x => x.IdPedido).ToListAsync());
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obtener(int id) => await db.Pedidos.AsNoTracking().Include(x => x.Detalles)
        .SingleOrDefaultAsync(x => x.IdPedido == id) is { } x ? Ok(x) : NotFound();
    [HttpGet("{id:int}/detalles")]
    public async Task<IActionResult> Detalles(int id)
    {
        if (!await db.Pedidos.AnyAsync(x => x.IdPedido == id)) return NotFound();
        return Ok(await db.Detalles.AsNoTracking().Where(x => x.IdPedido == id).ToListAsync());
    }
    [HttpPost]
    public async Task<IActionResult> Crear(PedidoSolicitud dto)
    {
        var detalles = await servicio.Preparar(dto);
        var x = new Pedido { IdCliente = dto.IdCliente, Detalles = detalles, Total = detalles.Sum(d => d.Cantidad * d.PrecioUnitario) };
        db.Pedidos.Add(x);
        // SaveChanges usa una transacción para cabecera y detalles.
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Obtener), new { id = x.IdPedido }, x);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, PedidoSolicitud dto)
    {
        if (!await db.Pedidos.AnyAsync(x => x.IdPedido == id)) return NotFound();
        var detalles = await servicio.Preparar(dto);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var x = await db.Pedidos.FromSqlInterpolated($"SELECT * FROM Pedido WITH (UPDLOCK, HOLDLOCK) WHERE IdPedido={id}").SingleAsync();
        if (x.Estado != "Pendiente") return Conflict(new { mensaje = "Solo se modifican pedidos pendientes." });
        await db.Entry(x).Collection(p => p.Detalles).LoadAsync();
        db.Detalles.RemoveRange(x.Detalles);
        await db.SaveChangesAsync();
        x.IdCliente = dto.IdCliente;
        x.Detalles = detalles;
        x.Total = detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return Ok(x);
    }
    [HttpPatch("{id:int}/confirmar")]
    public async Task<IActionResult> Confirmar(int id) => await CambiarEstado(id, "Confirmado");
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancelar(int id) => await CambiarEstado(id, "Cancelado");

    private async Task<IActionResult> CambiarEstado(int id, string estado)
    {
        // Actualización condicional atómica: dos solicitudes no cierran el mismo pedido.
        var cambios = await db.Pedidos.Where(x => x.IdPedido == id && x.Estado == "Pendiente")
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Estado, estado));
        if (cambios > 0) return NoContent();
        return await db.Pedidos.AnyAsync(x => x.IdPedido == id)
            ? Conflict(new { mensaje = "El pedido ya no está pendiente." }) : NotFound();
    }
}
