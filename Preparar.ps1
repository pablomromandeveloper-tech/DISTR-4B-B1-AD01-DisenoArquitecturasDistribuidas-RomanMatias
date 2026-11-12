$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet tool restore
    if ($LASTEXITCODE -ne 0) { throw 'No se pudieron restaurar las herramientas.' }
    foreach ($nombre in @('Catalogo', 'Inventario', 'Pedidos')) {
        dotnet build "$nombre\$nombre.sln"
        if ($LASTEXITCODE -ne 0) { throw "Error al compilar $nombre." }
        dotnet run --no-build --project "$nombre\$nombre.Api" -- --migrar
        if ($LASTEXITCODE -ne 0) { throw "No se pudo migrar la base de $nombre. Revisa SQL Server y la conexión." }
    }
    'Preparacion terminada. Ejecuta .\Iniciar.ps1 o abre las soluciones en Visual Studio.'
}
finally { Pop-Location }
