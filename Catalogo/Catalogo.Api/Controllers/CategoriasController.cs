using Catalogo.Api.Contracts;
using Catalogo.Api.Data;
using Catalogo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Api.Controllers;

[ApiController, Route("api/categorias")]
public class CategoriasController(CatalogoContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await db.Categorias.AsNoTracking().OrderBy(x => x.IdCategoria).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obtener(int id) => await db.Categorias.FindAsync(id) is { } x ? Ok(x) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Crear(CategoriaSolicitud dto)
    {
        var x = new Categoria { Nombre = dto.Nombre.Trim(), Descripcion = dto.Descripcion.Trim() };
        db.Categorias.Add(x);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Obtener), new { id = x.IdCategoria }, x);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, CategoriaSolicitud dto)
    {
        var x = await db.Categorias.FindAsync(id);
        if (x is null) return NotFound();
        x.Nombre = dto.Nombre.Trim();
        x.Descripcion = dto.Descripcion.Trim();
        await db.SaveChangesAsync();
        return Ok(x);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var x = await db.Categorias.FindAsync(id);
        if (x is null) return NotFound();
        if (await db.Productos.AnyAsync(p => p.IdCategoria == id)) return Conflict(new { mensaje = "La categoría tiene productos." });
        db.Categorias.Remove(x);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
