using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Pedidos.Api.Infrastructure;

public class ApiException(int estado, string mensaje) : Exception(mensaje)
{
    public int Estado { get; } = estado;
}
public class ErroresMiddleware(RequestDelegate next, ILogger<ErroresMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            var (estado, mensaje) = ex switch
            {
                ApiException e => (e.Estado, e.Message),
                DbUpdateException => (409, "Conflicto de datos: registro duplicado, relacionado o fuera de los límites permitidos."),
                SqlException { Number: 1205 } => (409, "Operación concurrente. Intenta nuevamente."),
                SqlException => (503, "No se pudo acceder a SQL Server."),
                HttpRequestException or TaskCanceledException => (503, "El servicio requerido no está disponible."),
                _ => (500, "Ocurrió un error interno.")
            };
            logger.LogError(ex, "Error HTTP {Estado}", estado);
            context.Response.StatusCode = estado;
            await context.Response.WriteAsJsonAsync(new ProblemDetails { Status = estado, Title = mensaje, Instance = context.Request.Path });
        }
    }
}
