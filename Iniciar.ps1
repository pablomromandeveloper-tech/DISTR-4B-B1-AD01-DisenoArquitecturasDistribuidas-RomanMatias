$ErrorActionPreference = 'Stop'
foreach ($nombre in @('Catalogo', 'Inventario', 'Pedidos')) {
    $proyecto = Join-Path $PSScriptRoot "$nombre\$nombre.Api"
    if (-not (Test-Path -LiteralPath (Join-Path $proyecto "bin\Debug\net10.0\$nombre.Api.dll"))) {
        throw 'Primero ejecuta .\Preparar.ps1 para compilar y crear las bases.'
    }
    $comando = "dotnet run --no-build --project '$proyecto' --launch-profile http"
    Start-Process powershell.exe -WorkingDirectory $proyecto -ArgumentList '-NoExit', '-Command', $comando
}
'Catálogo: http://localhost:5101/swagger'
'Inventario: http://localhost:5102/swagger'
'Pedidos: http://localhost:5103/swagger'
'Para detener las APIs, presiona Ctrl+C en cada ventana.'
