using Pedidos.Api.Contracts;
using Pedidos.Api.Data;
using Pedidos.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Pedidos.Api.Controllers;

[ApiController, Route("api/clientes")]
public class ClientesController(PedidosContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await db.Clientes.AsNoTracking().OrderBy(x => x.IdCliente).ToListAsync());
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obtener(int id) => await db.Clientes.FindAsync(id) is { } x ? Ok(x) : NotFound();
    [HttpPost]
    public async Task<IActionResult> Crear(ClienteSolicitud dto)
    {
        var x = new Cliente { Nombres = dto.Nombres.Trim(), Cedula = dto.Cedula, Correo = dto.Correo.Trim().ToLowerInvariant() };
        db.Clientes.Add(x);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Obtener), new { id = x.IdCliente }, x);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, ClienteSolicitud dto)
    {
        var x = await db.Clientes.FindAsync(id);
        if (x is null) return NotFound();
        x.Nombres = dto.Nombres.Trim(); x.Cedula = dto.Cedula; x.Correo = dto.Correo.Trim().ToLowerInvariant();
        await db.SaveChangesAsync();
        return Ok(x);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var x = await db.Clientes.FindAsync(id);
        if (x is null) return NotFound();
        if (await db.Pedidos.AnyAsync(p => p.IdCliente == id)) return Conflict(new { mensaje = "El cliente tiene pedidos." });
        db.Clientes.Remove(x);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
