using Catalogo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Api.Data;

public class CatalogoContext(DbContextOptions<CatalogoContext> options) : DbContext(options)
{
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Marca> Marcas => Set<Marca>();
    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder m)
    {
        m.Entity<Categoria>().ToTable("Categoria");
        m.Entity<Marca>().ToTable("Marca");
        m.Entity<Producto>().ToTable("Producto", t => t.HasCheckConstraint("CK_Producto_Precio", "[Precio] > 0"));
        m.Entity<Categoria>().HasIndex(x => x.Nombre).IsUnique();
        m.Entity<Marca>().HasIndex(x => x.Nombre).IsUnique();
        m.Entity<Producto>().HasIndex(x => x.SKU).IsUnique();
        m.Entity<Producto>().Property(x => x.Precio).HasPrecision(10, 2);
        m.Entity<Producto>().HasOne<Categoria>().WithMany().HasForeignKey(x => x.IdCategoria).OnDelete(DeleteBehavior.Restrict);
        m.Entity<Producto>().HasOne<Marca>().WithMany().HasForeignKey(x => x.IdMarca).OnDelete(DeleteBehavior.Restrict);
    }
}
