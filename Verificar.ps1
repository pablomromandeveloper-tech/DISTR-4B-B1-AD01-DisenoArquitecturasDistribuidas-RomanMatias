$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    & .\Preparar.ps1
    py -X utf8 .\scripts\verificar.py
    if ($LASTEXITCODE -ne 0) { throw 'Fallaron las pruebas. Revisa artifacts y docs/evidencias.' }
}
finally { Pop-Location }
