using Inventario.Api.Contracts;
using Inventario.Api.Data;
using Inventario.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Api.Controllers;

[ApiController, Route("api/bodegas")]
public class BodegasController(InventarioContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await db.Bodegas.AsNoTracking().OrderBy(x => x.IdBodega).ToListAsync());
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obtener(int id) => await db.Bodegas.FindAsync(id) is { } x ? Ok(x) : NotFound();
    [HttpPost]
    public async Task<IActionResult> Crear(BodegaSolicitud dto)
    {
        var x = new Bodega { Nombre = dto.Nombre.Trim(), Ciudad = dto.Ciudad.Trim(), Direccion = dto.Direccion.Trim() };
        db.Bodegas.Add(x);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Obtener), new { id = x.IdBodega }, x);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, BodegaSolicitud dto)
    {
        var x = await db.Bodegas.FindAsync(id);
        if (x is null) return NotFound();
        x.Nombre = dto.Nombre.Trim(); x.Ciudad = dto.Ciudad.Trim(); x.Direccion = dto.Direccion.Trim();
        await db.SaveChangesAsync();
        return Ok(x);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var x = await db.Bodegas.FindAsync(id);
        if (x is null) return NotFound();
        if (await db.Existencias.AnyAsync(e => e.IdBodega == id) || await db.Movimientos.AnyAsync(e => e.IdBodega == id)
            || await db.Reservas.AnyAsync(e => e.IdBodega == id)) return Conflict(new { mensaje = "La bodega tiene registros relacionados." });
        db.Bodegas.Remove(x);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
