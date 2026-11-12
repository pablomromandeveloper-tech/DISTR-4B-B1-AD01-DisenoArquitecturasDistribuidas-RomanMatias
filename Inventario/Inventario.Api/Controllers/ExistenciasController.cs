using Inventario.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Api.Controllers;

[ApiController, Route("api/existencias")]
public class ExistenciasController(InventarioContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int? idProducto, [FromQuery] int? idBodega)
    {
        var q = db.Existencias.AsNoTracking().AsQueryable();
        if (idProducto.HasValue) q = q.Where(x => x.IdProducto == idProducto);
        if (idBodega.HasValue) q = q.Where(x => x.IdBodega == idBodega);
        return Ok(await q.OrderBy(x => x.IdExistencia).Select(x => new
        {
            x.IdExistencia, x.IdProducto, x.IdBodega, x.Stock,
            Reservado = db.Reservas.Where(r => r.IdProducto == x.IdProducto && r.IdBodega == x.IdBodega && r.Estado == "Activa").Sum(r => (int?)r.Cantidad) ?? 0,
            Disponible = x.Stock - (db.Reservas.Where(r => r.IdProducto == x.IdProducto && r.IdBodega == x.IdBodega && r.Estado == "Activa").Sum(r => (int?)r.Cantidad) ?? 0)
        }).ToListAsync());
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obtener(int id) => await db.Existencias.FindAsync(id) is { } x ? Ok(x) : NotFound();
}
