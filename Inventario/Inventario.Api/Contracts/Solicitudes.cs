using System.ComponentModel.DataAnnotations;

namespace Inventario.Api.Contracts;

public record BodegaSolicitud(
    [Required, StringLength(100, MinimumLength = 2)] string Nombre,
    [Required, StringLength(80)] string Ciudad,
    [Required, StringLength(250)] string Direccion);
public record MovimientoSolicitud(
    [Range(1, int.MaxValue)] int IdProducto,
    [Range(1, int.MaxValue)] int IdBodega,
    [Required, RegularExpression("^(Entrada|Salida)$")] string Tipo,
    [Range(1, 1000000)] int Cantidad);
public record ReservaSolicitud(
    [Range(1, int.MaxValue)] int IdProducto,
    [Range(1, int.MaxValue)] int IdBodega,
    [Range(1, 1000000)] int Cantidad);
