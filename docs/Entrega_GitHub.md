# Publicación y entrega

## Nombre obligatorio del repositorio

```text
DISTR-4B-B1-AD01-DisenoArquitecturasDistribuidas-RomanMatias
```

Estudiante: **Pablo Matias Roman Ruiz**. Curso: **4.º B**. Nombre corto de entrega: **Matias Roman**. El nombre del repositorio usa `4B` y `RomanMatias`, conforme a los datos confirmados por el usuario.

Un repositorio puede contener las **tres soluciones independientes**. No es necesario crear un repositorio por cada API para cumplir la práctica.

## Enlaces de la entrega

- Cuenta autorizada: https://github.com/pablomromandeveloper-tech
- Repositorio: https://github.com/pablomromandeveloper-tech/DISTR-4B-B1-AD01-DisenoArquitecturasDistribuidas-RomanMatias
- Descargas finales, documentos y paquete ZIP: https://github.com/pablomromandeveloper-tech/DISTR-4B-B1-AD01-DisenoArquitecturasDistribuidas-RomanMatias/releases/tag/entrega-ad01

## Contenido que se entrega

- Las carpetas Catalogo, Inventario y Pedidos con código, `.sln`, `.csproj`, configuración y migraciones.
- La colección de Postman de la carpeta postman.
- Scripts SQL y scripts PowerShell para ejecutar la práctica.
- README y documentación, incluida `docs/Registro_IA_Matias_Roman.pdf` y su Word editable.
- Evidencias reales de ejecución y espacio para registrar la revisión personal en la declaración.

## Pasos de publicación

La carpeta está preparada con `.gitignore`. Después de revisar tus datos, desde su raíz:

```powershell
git status
git diff
git add .gitignore README.md Catalogo Inventario Pedidos postman sql docs scripts .config Preparar.ps1 Iniciar.ps1 Verificar.ps1
git diff --cached
git commit -m "Implementa microservicios de catalogo inventario y pedidos"
```

Crea un repositorio vacío en GitHub con el nombre correcto. Sustituye los valores siguientes:

```powershell
git branch -M main
git remote add origin https://github.com/pablomromandeveloper-tech/DISTR-4B-B1-AD01-DisenoArquitecturasDistribuidas-RomanMatias.git
git push -u origin main
```

En Visual Studio también puedes usar **Git > Crear repositorio de Git** y publicarlo en GitHub.

## Revisión final de la entrega

1. El enlace de GitHub permite consultar las tres soluciones.
2. `bin`, `obj`, `.vs` y `artifacts` no aparecen como archivos versionados.
3. La colección se importa y ejecuta en Postman.
4. El PDF de declaración refleja el apoyo real y tu revisión personal.
5. El nivel/paralelo del repositorio es el correcto.

La cuenta se vinculó mediante el flujo oficial de autorización de GitHub CLI. El autor corto de la entrega es Matias Roman; no se incluyen credenciales en el repositorio.
