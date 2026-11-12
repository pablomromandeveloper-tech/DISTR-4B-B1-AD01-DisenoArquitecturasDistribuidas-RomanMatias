using System.Net;
using Inventario.Api.Infrastructure;

namespace Inventario.Api.Services;

public class CatalogoClient(HttpClient http)
{
    private record ProductoRemoto(int IdProducto, bool Estado);

    public async Task ValidarProducto(int id)
    {
        using var r = await http.GetAsync($"api/productos/{id}");
        if (r.StatusCode == HttpStatusCode.NotFound) throw new ApiException(400, "El producto no existe en Catálogo.");
        if (!r.IsSuccessStatusCode) throw new ApiException(503, "Catálogo no pudo validar el producto.");
        var p = await r.Content.ReadFromJsonAsync<ProductoRemoto>();
        if (p is null || !p.Estado) throw new ApiException(400, "El producto está inactivo.");
    }
}
