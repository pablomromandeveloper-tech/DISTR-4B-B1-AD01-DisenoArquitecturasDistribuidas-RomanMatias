using System.ComponentModel.DataAnnotations;

namespace Catalogo.Api.Contracts;

public record CategoriaSolicitud(
    [Required, StringLength(100, MinimumLength = 2)] string Nombre,
    [Required, StringLength(300)] string Descripcion);

public record MarcaSolicitud(
    [Required, StringLength(100, MinimumLength = 2)] string Nombre,
    [Required, StringLength(80)] string PaisOrigen);

public record ProductoSolicitud(
    [Range(1, int.MaxValue)] int IdCategoria,
    [Range(1, int.MaxValue)] int IdMarca,
    [Required, StringLength(150, MinimumLength = 2)] string Nombre,
    [Required, RegularExpression(@"^[A-Za-z0-9\-]{3,40}$")] string SKU,
    [Range(typeof(decimal), "0.01", "99999999.99", ParseLimitsInInvariantCulture = true)] decimal Precio,
    [Required] bool? Estado);
