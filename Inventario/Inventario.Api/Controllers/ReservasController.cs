using Inventario.Api.Contracts;
using Inventario.Api.Data;
using Inventario.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Api.Controllers;

[ApiController, Route("api/reservas")]
public class ReservasController(InventarioContext db, StockService stock) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await db.Reservas.AsNoTracking().OrderBy(x => x.IdReserva).ToListAsync());
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obtener(int id) => await db.Reservas.FindAsync(id) is { } x ? Ok(x) : NotFound();
    [HttpPost]
    public async Task<IActionResult> Crear(ReservaSolicitud dto)
    {
        var x = await stock.Reservar(dto);
        return CreatedAtAction(nameof(Obtener), new { id = x.IdReserva }, x);
    }
    [HttpPatch("{id:int}/cancelar")]
    public async Task<IActionResult> Cancelar(int id) => Ok(await stock.CerrarReserva(id, false));
    [HttpPatch("{id:int}/consumir")]
    public async Task<IActionResult> Consumir(int id) => Ok(await stock.CerrarReserva(id, true));
}
