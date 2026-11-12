using Catalogo.Api.Contracts;
using Catalogo.Api.Data;
using Catalogo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Api.Controllers;

[ApiController, Route("api/productos")]
public class ProductosController(CatalogoContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await db.Productos.AsNoTracking().OrderBy(x => x.IdProducto).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obtener(int id) => await db.Productos.FindAsync(id) is { } x ? Ok(x) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Crear(ProductoSolicitud dto)
    {
        if (!await ReferenciasValidas(dto)) return BadRequest(new { mensaje = "La categoría o la marca no existe." });
        if (decimal.Round(dto.Precio, 2) != dto.Precio) return BadRequest(new { mensaje = "El precio admite dos decimales." });
        var x = new Producto();
        Asignar(x, dto);
        db.Productos.Add(x);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Obtener), new { id = x.IdProducto }, x);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, ProductoSolicitud dto)
    {
        var x = await db.Productos.FindAsync(id);
        if (x is null) return NotFound();
        if (!await ReferenciasValidas(dto)) return BadRequest(new { mensaje = "La categoría o la marca no existe." });
        if (decimal.Round(dto.Precio, 2) != dto.Precio) return BadRequest(new { mensaje = "El precio admite dos decimales." });
        Asignar(x, dto);
        await db.SaveChangesAsync();
        return Ok(x);
    }

    // Baja lógica: otros servicios conservan referencias e historial del producto.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Desactivar(int id)
    {
        var x = await db.Productos.FindAsync(id);
        if (x is null) return NotFound();
        x.Estado = false;
        await db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<bool> ReferenciasValidas(ProductoSolicitud d) =>
        await db.Categorias.AnyAsync(x => x.IdCategoria == d.IdCategoria) && await db.Marcas.AnyAsync(x => x.IdMarca == d.IdMarca);

    private static void Asignar(Producto x, ProductoSolicitud d)
    {
        x.IdCategoria = d.IdCategoria;
        x.IdMarca = d.IdMarca;
        x.Nombre = d.Nombre.Trim();
        x.SKU = d.SKU.ToUpperInvariant();
        x.Precio = d.Precio;
        x.Estado = d.Estado!.Value;
    }
}
