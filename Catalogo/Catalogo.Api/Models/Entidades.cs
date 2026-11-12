using System.ComponentModel.DataAnnotations;

namespace Catalogo.Api.Models;

public class Categoria
{
    [Key] public int IdCategoria { get; set; }
    [MaxLength(100)] public string Nombre { get; set; } = "";
    [MaxLength(300)] public string Descripcion { get; set; } = "";
}

public class Marca
{
    [Key] public int IdMarca { get; set; }
    [MaxLength(100)] public string Nombre { get; set; } = "";
    [MaxLength(80)] public string PaisOrigen { get; set; } = "";
}

public class Producto
{
    [Key] public int IdProducto { get; set; }
    public int IdCategoria { get; set; }
    public int IdMarca { get; set; }
    [MaxLength(150)] public string Nombre { get; set; } = "";
    [MaxLength(40)] public string SKU { get; set; } = "";
    public decimal Precio { get; set; }
    public bool Estado { get; set; } = true;
}
