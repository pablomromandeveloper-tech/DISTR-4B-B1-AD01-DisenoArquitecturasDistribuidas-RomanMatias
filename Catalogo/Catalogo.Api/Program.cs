using Catalogo.Api.Data;
using Catalogo.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CatalogoContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
app.UseMiddleware<ErroresMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
app.MapGet("/health", async (CatalogoContext db) =>
    await db.Database.CanConnectAsync() ? Results.Ok(new { servicio = "Catalogo.Api", estado = "OK" })
        : Results.Problem("SQL Server no disponible.", statusCode: 503));

// La creación de esquema se ejecuta explícitamente, no en cada petición.
if (args.Contains("--migrar"))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<CatalogoContext>().Database.MigrateAsync();
    return;
}
app.Run();
