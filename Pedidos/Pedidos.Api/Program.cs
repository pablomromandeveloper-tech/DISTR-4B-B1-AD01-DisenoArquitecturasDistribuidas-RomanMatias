using Pedidos.Api.Data;
using Pedidos.Api.Infrastructure;
using Pedidos.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PedidosContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient("Catalogo", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Servicios:Catalogo"]!);
    c.Timeout = TimeSpan.FromSeconds(5);
});
builder.Services.AddHttpClient("Inventario", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Servicios:Inventario"]!);
    c.Timeout = TimeSpan.FromSeconds(5);
});
builder.Services.AddScoped<PedidoService>();
var app = builder.Build();
app.UseMiddleware<ErroresMiddleware>();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();
app.MapGet("/health", async (PedidosContext db) =>
    await db.Database.CanConnectAsync() ? Results.Ok(new { servicio = "Pedidos.Api", estado = "OK" })
        : Results.Problem("SQL Server no disponible.", statusCode: 503));
if (args.Contains("--migrar"))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<PedidosContext>().Database.MigrateAsync();
    return;
}
app.Run();
