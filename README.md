# Comercio multibodega — AD01

**Instituto Superior Tecnológico Cordillera** · Desarrollo de Software

Aplicaciones Distribuidas — Unidad 1 — Práctica 1

**Estudiante:** Pablo Matias Roman Ruiz.

**Nivel/paralelo:** 4.º B.

**Nombre corto:** Matias Roman.

Implementación académica de **tres microservicios REST independientes**, con ASP.NET Core Web API, Entity Framework Core y SQL Server. Cada solución de Visual Studio tiene su propio proyecto, proceso, configuración, migraciones y base de datos.

## Entrega y documentos finales

- **Repositorio:** https://github.com/pablomromandeveloper-tech/DISTR-4B-B1-AD01-DisenoArquitecturasDistribuidas-RomanMatias
- **Paquete de entrega y descargas:** https://github.com/pablomromandeveloper-tech/DISTR-4B-B1-AD01-DisenoArquitecturasDistribuidas-RomanMatias/releases/tag/entrega-ad01
- [Informe técnico en PDF](docs/Informe_Tecnico.pdf).
- [Declaración de uso de IA en PDF](docs/Registro_IA_Matias_Roman.pdf).
- [Declaración editable en Word](docs/Registro_IA_Matias_Roman.docx).
- [Colección Postman](postman/ComercioMultibodega.postman_collection.json).
- [Evidencias de pruebas](docs/evidencias/resultado-integracion.json).

## 1. Soluciones que debes abrir en Visual Studio

| Solución | Proyecto | Base de datos | Swagger |
|---|---|---|---|
| `Catalogo/Catalogo.sln` | `Catalogo.Api` | `CatalogoDB` | http://localhost:5101/swagger |
| `Inventario/Inventario.sln` | `Inventario.Api` | `InventarioDB` | http://localhost:5102/swagger |
| `Pedidos/Pedidos.sln` | `Pedidos.Api` | `PedidosDB` | http://localhost:5103/swagger |

Abre cada `.sln` en una ventana diferente de Visual Studio. Selecciona el perfil **http** y ejecuta los tres proyectos. No hay referencias de proyecto entre los servicios.

## 2. Requisitos y preparación

- Visual Studio con soporte para .NET 10 y la carga de trabajo ASP.NET y desarrollo web.
- SDK .NET 10; SQL Server 2019 o posterior, con permisos para crear bases en el entorno local.
- Postman para probar manualmente.
- Solo para la verificación automática: Python 3 y Node.js/npm. La primera ejecución descarga Newman 6.2.3.

Las conexiones predeterminadas usan la instancia SQL Server local `.` y autenticación de Windows, que se comprobaron en este equipo. No se incluyen contraseñas.

Desde PowerShell, dentro de esta carpeta:

```powershell
.\Preparar.ps1
.\Iniciar.ps1
```

`Preparar.ps1` restaura herramientas, compila cada solución y aplica sus migraciones. Es repetible: no borra datos. `Iniciar.ps1` abre una consola por API. Para detenerlas, presiona **Ctrl+C** en cada consola.

También puedes preparar cada proyecto directamente:

```powershell
dotnet tool restore
dotnet ef database update --project Catalogo/Catalogo.Api
dotnet ef database update --project Inventario/Inventario.Api
dotnet ef database update --project Pedidos/Pedidos.Api
```

Si usas una instancia nombrada, cambia `ConnectionStrings:DefaultConnection` en el `appsettings.json` de **cada servicio**, manteniendo los tres nombres de base distintos. Ejemplo: `Server=.\\SQLEXPRESS;Database=CatalogoDB;Trusted_Connection=True;TrustServerCertificate=True` dentro de JSON. Las URLs entre servicios están en la sección `Servicios`.

## 3. Cómo probar con Postman

1. Ejecuta las tres APIs.
2. Importa `postman/ComercioMultibodega.postman_collection.json`.
3. Ejecuta **toda la colección, en orden**, desde Collection Runner.
4. La primera petición genera un sufijo único; las siguientes guardan automáticamente los IDs reales. No necesitas cambiar identificadores a mano.
5. Revisa los resultados y exporta el reporte del Runner para tu evidencia personal.

La colección contiene **90 peticiones y 98 aserciones**. Crea categoría, marca, producto, bodega, movimientos, reservas, cliente y pedidos. Comprueba stock, precios, total, cambios de estado, actualizaciones, bajas, errores 400, inexistentes 404 y conflictos 409. Los datos son ficticios y quedan disponibles para la demostración.

### Verificación automática completa

Cierra previamente las tres APIs; este script inicia y detiene sus propios procesos:

```powershell
.\Verificar.ps1
```

Además de ejecutar la colección mediante Newman, comprueba reservas simultáneas, consumo/cancelación doble, entradas concurrentes y servicios caídos. Resultado resumido: `docs/evidencias/resultado-integracion.json`. Reporte completo local: `artifacts/newman.json`.

**Resultado observado el 11/11/2026:** tres soluciones compiladas sin errores ni advertencias, 90 peticiones y 98 aserciones correctas, y 13 comprobaciones de integración correctas (la primera corresponde a la colección completa). Se utilizó SQL Server real.

## 4. Organización sencilla del código

```text
Catalogo/    Catalogo.sln + Catalogo.Api/
Inventario/  Inventario.sln + Inventario.Api/
Pedidos/     Pedidos.sln + Pedidos.Api/
  Controllers/      Rutas y respuestas HTTP
  Contracts/        DTO de entrada y validaciones
  Models/           Entidades de la base propia
  Data/             DbContext y reglas SQL
  Services/         Reglas de stock/pedidos y llamadas HTTP donde corresponda
  Infrastructure/   Manejo de errores
  Migrations/       Esquema versionado de cada base
postman/            Colección importable
sql/                Scripts de creación y migración para revisión en SSMS
docs/               Informe, fuentes, bitácora, PDF y sustentación
scripts/            Generación de documentos y verificación
```

## 5. Reglas importantes para explicar

- **Catálogo:** categoría y marca son FK físicas de Producto. SKU único, precio positivo con dos decimales y estado booleano. DELETE de producto realiza baja lógica para conservar referencias históricas.
- **Inventario:** Bodega es FK física de existencias, movimientos y reservas. IdProducto es referencia lógica validada por HTTP contra Catálogo. Existe una única fila por producto/bodega.
- **Stock:** una entrada suma y una salida resta. `Disponible = Stock - reservas activas`. No se modifica Stock con un CRUD directo: todo cambio deja un movimiento. Los movimientos son inmutables; una corrección se representa con otro movimiento.
- **Reservas:** crear una reserva disminuye disponible, no stock físico. Cancelar la libera. Consumirla descuenta stock y crea una salida en la misma transacción. Los bloqueos SQL evitan consumir o reservar dos veces las mismas unidades.
- **Pedidos:** Cliente y DetallePedido tienen relaciones físicas dentro de PedidosDB. Cada producto se valida vía Catálogo y se consulta su disponibilidad en Inventario. PrecioUnitario se toma del catálogo y se guarda como histórico; Total se calcula en el servidor. No se reciben totales ni precios del cliente.
- **Alcance del pedido:** crear/editar un pedido comprueba disponibilidad en ese momento, pero **no reserva ni descuenta stock automáticamente**. Confirmar es un estado administrativo. Las reservas y el despacho físico se ejecutan explícitamente en Inventario. Por eso varios pedidos pendientes pueden consultar el mismo stock; no se presenta esta consulta como una garantía de asignación. La automatización pedido–reserva sería una ampliación mediante orquestación y compensaciones.
- **Estados:** pedidos Pendiente → Confirmado o Cancelado; solo se editan pendientes. Reservas Activa → Cancelada o Consumida. Las transiciones finales no se repiten.
- **Independencia:** cada API consulta su propia base. Si Catálogo cae, Inventario y Pedidos conservan consultas locales, y rechazan con 503 las escrituras que necesitan validación remota.

## 6. Documentos de entrega

- `docs/Informe_Tecnico.pdf` y `docs/Informe_Tecnico.md`.
- `docs/Registro_IA_Matias_Roman.pdf` y `.docx`, preparado a partir de tu plantilla.
- `docs/Bitacora_IA.md`, con el prompt relevante literal, uso aprovechado/descartado y pendientes personales.
- `docs/Fuentes_Verificadas.md`, con cinco documentos oficiales consultados y su aplicación.
- `docs/Guia_Sustentacion.md`, preguntas y recorrido sugerido.
- `docs/Entrega_GitHub.md`, formato del repositorio y pasos de publicación.

La declaración identifica el apoyo real de IA y las pruebas realizadas por el asistente. Incluye el nombre y curso confirmados. **Antes de entregarla, registra tu revisión personal del código, tu ejecución de la colección y tu firma.** No se afirma que esa revisión del estudiante ya ocurrió.

Para volver a generar los documentos después de editar el informe o la bitácora:

```powershell
py -m pip install python-docx reportlab
py scripts/generar_documentos.py --plantilla "C:\Users\MSI\Downloads\Plantilla_Registro_Prompts_y_Fuentes.docx"
```

Para regenerar la colección desde su definición: `py scripts/generar_postman.py`. Los PDF y el DOCX ya están incluidos; Python y estas librerías no son requisitos para abrirlos ni para ejecutar las APIs.

## 7. Versionamiento

`.gitignore` excluye `bin`, `obj`, `.vs`, archivos personales y logs. El código, migraciones, SQL, Postman y documentos sí deben subirse.

Formato solicitado por el documento docente:

```text
DISTR-4B-B1-AD01-DisenoArquitecturasDistribuidas-RomanMatias
```

El segmento `4B` corresponde a 4.º B y `RomanMatias` al nombre corto Matias Roman. La carpeta de entrega utiliza el mismo nombre del repositorio.

## 8. Problemas frecuentes

- **No conecta a SQL Server:** verifica el servicio de la instancia y las tres cadenas de conexión; luego ejecuta `Preparar.ps1`.
- **503 al registrar movimientos o pedidos:** inicia primero Catálogo, luego Inventario y finalmente Pedidos; revisa las URLs de `Servicios`.
- **409 al borrar un maestro:** tiene registros relacionados; se conserva la integridad y el historial.
- **Puerto ocupado al verificar:** detén las ventanas de Visual Studio/consolas que estén ejecutando estas APIs.
- **Cambiaste el modelo:** genera una nueva migración en el proyecto correspondiente y aplícala; no borres una base que contiene trabajo útil.

## Fuentes

Las decisiones se fundamentan en documentación oficial de Microsoft y Postman [1–5], detallada y enlazada en `docs/Fuentes_Verificadas.md`.
