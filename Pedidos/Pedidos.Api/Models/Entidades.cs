using System.ComponentModel.DataAnnotations;

namespace Pedidos.Api.Models;

public class Cliente
{
    [Key] public int IdCliente { get; set; }
    [MaxLength(150)] public string Nombres { get; set; } = "";
    [MaxLength(10)] public string Cedula { get; set; } = "";
    [MaxLength(150)] public string Correo { get; set; } = "";
}
public class Pedido
{
    [Key] public int IdPedido { get; set; }
    public int IdCliente { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public decimal Total { get; set; }
    [MaxLength(15)] public string Estado { get; set; } = "Pendiente";
    public List<DetallePedido> Detalles { get; set; } = [];
}
public class DetallePedido
{
    [Key] public int IdDetalle { get; set; }
    public int IdPedido { get; set; }
    public int IdProducto { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
