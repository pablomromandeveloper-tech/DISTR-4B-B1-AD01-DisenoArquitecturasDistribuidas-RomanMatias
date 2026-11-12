using Inventario.Api.Contracts;
using Inventario.Api.Data;
using Inventario.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Api.Controllers;

[ApiController, Route("api/movimientos")]
public class MovimientosController(InventarioContext db, StockService stock) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await db.Movimientos.AsNoTracking().OrderBy(x => x.IdMovimiento).ToListAsync());
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obtener(int id) => await db.Movimientos.FindAsync(id) is { } x ? Ok(x) : NotFound();
    [HttpPost]
    public async Task<IActionResult> Crear(MovimientoSolicitud dto)
    {
        var x = await stock.Mover(dto);
        return CreatedAtAction(nameof(Obtener), new { id = x.IdMovimiento }, x);
    }
}
