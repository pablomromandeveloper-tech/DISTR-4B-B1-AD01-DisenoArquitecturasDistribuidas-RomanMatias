using System.Net;
using Pedidos.Api.Contracts;
using Pedidos.Api.Data;
using Pedidos.Api.Infrastructure;
using Pedidos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Pedidos.Api.Services;

public class PedidoService(PedidosContext db, IHttpClientFactory clients)
{
    private record ProductoRemoto(int IdProducto, decimal Precio, bool Estado);
    private record ExistenciaRemota(int IdBodega, int Disponible);

    public async Task<List<DetallePedido>> Preparar(PedidoSolicitud dto)
    {
        if (!await db.Clientes.AnyAsync(x => x.IdCliente == dto.IdCliente)) throw new ApiException(400, "El cliente no existe.");
        if (dto.Detalles.Any(x => x is null)) throw new ApiException(400, "Las líneas del pedido no pueden ser nulas.");
        if (dto.Detalles.Select(x => x.IdProducto).Distinct().Count() != dto.Detalles.Count)
            throw new ApiException(400, "No repitas productos: agrupa su cantidad en una sola línea.");
        var detalles = new List<DetallePedido>();
        foreach (var linea in dto.Detalles)
        {
            using var r = await clients.CreateClient("Catalogo").GetAsync($"api/productos/{linea.IdProducto}");
            if (r.StatusCode == HttpStatusCode.NotFound) throw new ApiException(400, $"El producto {linea.IdProducto} no existe.");
            if (!r.IsSuccessStatusCode) throw new ApiException(503, "Catálogo no está disponible.");
            var producto = await r.Content.ReadFromJsonAsync<ProductoRemoto>();
            if (producto is null || !producto.Estado) throw new ApiException(400, "Uno de los productos está inactivo.");
            using var inventario = await clients.CreateClient("Inventario").GetAsync($"api/existencias?idProducto={linea.IdProducto}");
            if (!inventario.IsSuccessStatusCode) throw new ApiException(503, "Inventario no está disponible.");
            var existencias = await inventario.Content.ReadFromJsonAsync<List<ExistenciaRemota>>() ?? [];
            if (existencias.Sum(x => (long)x.Disponible) < linea.Cantidad)
                throw new ApiException(409, $"No hay disponibilidad suficiente para el producto {linea.IdProducto}.");
            detalles.Add(new DetallePedido { IdProducto = linea.IdProducto, Cantidad = linea.Cantidad, PrecioUnitario = producto.Precio });
        }
        return detalles;
    }
}
