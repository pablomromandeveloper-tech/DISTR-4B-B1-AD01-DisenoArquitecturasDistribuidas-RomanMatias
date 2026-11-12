using System.ComponentModel.DataAnnotations;

namespace Pedidos.Api.Contracts;

public record ClienteSolicitud(
    [Required, StringLength(150, MinimumLength = 3)] string Nombres,
    [Required, RegularExpression(@"^\d{10}$", ErrorMessage = "La cédula debe tener 10 dígitos.")] string Cedula,
    [Required, EmailAddress, StringLength(150)] string Correo);

public record LineaSolicitud(
    [Range(1, int.MaxValue)] int IdProducto,
    [Range(1, 1000000)] int Cantidad);

public record PedidoSolicitud(
    [Range(1, int.MaxValue)] int IdCliente,
    [Required, MinLength(1), MaxLength(50)] List<LineaSolicitud> Detalles);
