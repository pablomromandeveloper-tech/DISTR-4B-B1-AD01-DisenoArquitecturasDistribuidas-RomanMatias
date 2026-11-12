# Esquema SQL Server

La vía habitual es ejecutar `Preparar.ps1`, que aplica las migraciones EF Core de cada API.

Para revisar/crear el esquema desde SSMS:

1. Ejecuta `00_CrearBases.sql` en la instancia local.
2. Selecciona **CatalogoDB** y ejecuta `CatalogoDB.sql`.
3. Selecciona **InventarioDB** y ejecuta `InventarioDB.sql`.
4. Selecciona **PedidosDB** y ejecuta `PedidosDB.sql`.

Los tres scripts de esquema se generaron desde las migraciones usando `--idempotent`. Deben ejecutarse en su base correspondiente; no hacen referencias a otra base.

Con sqlcmd, desde la raíz del repositorio:

```powershell
sqlcmd -S . -E -C -b -i sql/00_CrearBases.sql
sqlcmd -S . -E -C -b -d CatalogoDB -i sql/CatalogoDB.sql
sqlcmd -S . -E -C -b -d InventarioDB -i sql/InventarioDB.sql
sqlcmd -S . -E -C -b -d PedidosDB -i sql/PedidosDB.sql
```

Para volver a generar un script después de nuevas migraciones:

```powershell
dotnet ef migrations script --idempotent --project Catalogo/Catalogo.Api --output sql/CatalogoDB.sql
```

No hay un INSERT directo de saldos. Los datos ficticios de demostración se crean al ejecutar la colección Postman, que conserva las reglas y registra los movimientos por la API.
