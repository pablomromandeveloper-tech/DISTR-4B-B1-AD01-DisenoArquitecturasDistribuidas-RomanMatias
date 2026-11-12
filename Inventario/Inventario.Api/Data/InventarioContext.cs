using Inventario.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Api.Data;

public class InventarioContext(DbContextOptions<InventarioContext> options) : DbContext(options)
{
    public DbSet<Bodega> Bodegas => Set<Bodega>();
    public DbSet<Existencia> Existencias => Set<Existencia>();
    public DbSet<MovimientoInventario> Movimientos => Set<MovimientoInventario>();
    public DbSet<ReservaStock> Reservas => Set<ReservaStock>();

    protected override void OnModelCreating(ModelBuilder m)
    {
        m.Entity<Bodega>().ToTable("Bodega");
        m.Entity<Bodega>().HasIndex(x => x.Nombre).IsUnique();
        m.Entity<Existencia>().ToTable("Existencia", t => t.HasCheckConstraint("CK_Existencia_Stock", "[Stock] >= 0"));
        m.Entity<Existencia>().HasIndex(x => new { x.IdProducto, x.IdBodega }).IsUnique();
        m.Entity<MovimientoInventario>().ToTable("MovimientoInventario", t =>
        {
            t.HasCheckConstraint("CK_Movimiento_Cantidad", "[Cantidad] > 0");
            t.HasCheckConstraint("CK_Movimiento_Tipo", "[Tipo] IN ('Entrada','Salida')");
        });
        m.Entity<ReservaStock>().ToTable("ReservaStock", t =>
        {
            t.HasCheckConstraint("CK_Reserva_Cantidad", "[Cantidad] > 0");
            t.HasCheckConstraint("CK_Reserva_Estado", "[Estado] IN ('Activa','Cancelada','Consumida')");
        });
        m.Entity<ReservaStock>().HasIndex(x => new { x.IdProducto, x.IdBodega, x.Estado });
        m.Entity<Existencia>().HasOne<Bodega>().WithMany().HasForeignKey(x => x.IdBodega).OnDelete(DeleteBehavior.Restrict);
        m.Entity<MovimientoInventario>().HasOne<Bodega>().WithMany().HasForeignKey(x => x.IdBodega).OnDelete(DeleteBehavior.Restrict);
        m.Entity<ReservaStock>().HasOne<Bodega>().WithMany().HasForeignKey(x => x.IdBodega).OnDelete(DeleteBehavior.Restrict);
        // IdProducto es una referencia lógica: CatalogoDB pertenece a otro servicio.
    }
}
