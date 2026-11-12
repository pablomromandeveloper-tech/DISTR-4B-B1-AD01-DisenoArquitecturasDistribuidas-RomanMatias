using Catalogo.Api.Contracts;
using Catalogo.Api.Data;
using Catalogo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Api.Controllers;

[ApiController, Route("api/marcas")]
public class MarcasController(CatalogoContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await db.Marcas.AsNoTracking().OrderBy(x => x.IdMarca).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obtener(int id) => await db.Marcas.FindAsync(id) is { } x ? Ok(x) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Crear(MarcaSolicitud dto)
    {
        var x = new Marca { Nombre = dto.Nombre.Trim(), PaisOrigen = dto.PaisOrigen.Trim() };
        db.Marcas.Add(x);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Obtener), new { id = x.IdMarca }, x);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, MarcaSolicitud dto)
    {
        var x = await db.Marcas.FindAsync(id);
        if (x is null) return NotFound();
        x.Nombre = dto.Nombre.Trim();
        x.PaisOrigen = dto.PaisOrigen.Trim();
        await db.SaveChangesAsync();
        return Ok(x);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var x = await db.Marcas.FindAsync(id);
        if (x is null) return NotFound();
        if (await db.Productos.AnyAsync(p => p.IdMarca == id)) return Conflict(new { mensaje = "La marca tiene productos." });
        db.Marcas.Remove(x);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
