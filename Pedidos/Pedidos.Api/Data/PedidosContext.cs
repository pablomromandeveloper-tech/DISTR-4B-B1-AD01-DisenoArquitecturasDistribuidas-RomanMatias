using Pedidos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Pedidos.Api.Data;

public class PedidosContext(DbContextOptions<PedidosContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> Detalles => Set<DetallePedido>();

    protected override void OnModelCreating(ModelBuilder m)
    {
        m.Entity<Cliente>().ToTable("Cliente");
        m.Entity<Cliente>().HasIndex(x => x.Cedula).IsUnique();
        m.Entity<Cliente>().HasIndex(x => x.Correo).IsUnique();
        m.Entity<Pedido>().ToTable("Pedido", t =>
        {
            t.HasCheckConstraint("CK_Pedido_Total", "[Total] > 0");
            t.HasCheckConstraint("CK_Pedido_Estado", "[Estado] IN ('Pendiente','Confirmado','Cancelado')");
        });
        m.Entity<Pedido>().Property(x => x.Total).HasPrecision(18, 2);
        m.Entity<Pedido>().HasOne<Cliente>().WithMany().HasForeignKey(x => x.IdCliente).OnDelete(DeleteBehavior.Restrict);
        m.Entity<DetallePedido>().ToTable("DetallePedido", t =>
        {
            t.HasCheckConstraint("CK_Detalle_Cantidad", "[Cantidad] > 0");
            t.HasCheckConstraint("CK_Detalle_Precio", "[PrecioUnitario] > 0");
        });
        m.Entity<DetallePedido>().Property(x => x.PrecioUnitario).HasPrecision(10, 2);
        m.Entity<DetallePedido>().HasIndex(x => new { x.IdPedido, x.IdProducto }).IsUnique();
        m.Entity<Pedido>().HasMany(x => x.Detalles).WithOne().HasForeignKey(x => x.IdPedido).OnDelete(DeleteBehavior.Cascade);
        // No se crea una FK hacia CatalogoDB. El precio se conserva como dato histórico.
    }
}
