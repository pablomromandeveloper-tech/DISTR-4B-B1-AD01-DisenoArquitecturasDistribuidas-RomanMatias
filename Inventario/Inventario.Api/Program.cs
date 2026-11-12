using Inventario.Api.Data;
using Inventario.Api.Infrastructure;
using Inventario.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<InventarioContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient<CatalogoClient>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Servicios:Catalogo"]!);
    c.Timeout = TimeSpan.FromSeconds(5);
});
builder.Services.AddScoped<StockService>();
var app = builder.Build();
app.UseMiddleware<ErroresMiddleware>();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();
app.MapGet("/health", async (InventarioContext db) =>
    await db.Database.CanConnectAsync() ? Results.Ok(new { servicio = "Inventario.Api", estado = "OK" })
        : Results.Problem("SQL Server no disponible.", statusCode: 503));
if (args.Contains("--migrar"))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<InventarioContext>().Database.MigrateAsync();
    return;
}
app.Run();
