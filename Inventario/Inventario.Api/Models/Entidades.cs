using System.ComponentModel.DataAnnotations;

namespace Inventario.Api.Models;

public class Bodega
{
    [Key] public int IdBodega { get; set; }
    [MaxLength(100)] public string Nombre { get; set; } = "";
    [MaxLength(80)] public string Ciudad { get; set; } = "";
    [MaxLength(250)] public string Direccion { get; set; } = "";
}
public class Existencia
{
    [Key] public int IdExistencia { get; set; }
    public int IdProducto { get; set; }
    public int IdBodega { get; set; }
    public int Stock { get; set; }
}
public class MovimientoInventario
{
    [Key] public int IdMovimiento { get; set; }
    public int IdProducto { get; set; }
    public int IdBodega { get; set; }
    [MaxLength(10)] public string Tipo { get; set; } = "";
    public int Cantidad { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
public class ReservaStock
{
    [Key] public int IdReserva { get; set; }
    public int IdProducto { get; set; }
    public int IdBodega { get; set; }
    public int Cantidad { get; set; }
    [MaxLength(15)] public string Estado { get; set; } = "Activa";
}
