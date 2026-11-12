# Fuentes consultadas y verificadas

Consulta realizada por el asistente el **11 de noviembre de 2026**. Se abrieron las cinco URLs y se contrastó el contenido con las decisiones descritas. El estudiante debe abrirlas y registrar su propia comprobación antes de firmar la declaración. Las fechas corresponden a las páginas consultadas, no a una fecha de publicación inventada del libro completo.

## [1] APIs y validaciones

**Autor institucional:** Microsoft.

**Título:** Create web APIs with ASP.NET Core.

**Fecha de la página (`ms.date`):** 6 de mayo de 2026.

**URL:** https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-10.0

Se verificaron las secciones ControllerBase, ApiController attribute y Automatic HTTP 400 responses. Respaldan el uso de controladores, rutas HTTP, `CreatedAtAction`, validación automática y respuestas 400/404. Aplicación: Controllers y DTO de las tres APIs.

## [2] EF Core y SQL Server

**Autor institucional:** Microsoft (mantenedor del proveedor: proyecto Entity Framework Core).

**Título:** Microsoft SQL Server Database Provider — EF Core.

**Fecha de la página:** 9 de junio de 2026.

**URL:** https://learn.microsoft.com/en-us/ef/core/providers/sql-server/

Se verificaron Install y Usage and configuration. Respaldan el paquete `Microsoft.EntityFrameworkCore.SqlServer`, `UseSqlServer` y el registro de DbContext con inyección de dependencias. Aplicación: Program.cs y Data de cada servicio.

## [3] Base de datos por microservicio

**Autor institucional:** Microsoft.

**Título:** Data sovereignty per microservice. Extracto del libro oficial .NET Microservices: Architecture for Containerized .NET Applications.

**Fecha indicada en la página:** 20 de septiembre de 2018; actualización de metadatos: 13 de abril de 2022.

**URL:** https://learn.microsoft.com/en-us/dotnet/architecture/microservices/architect-microservice-container-applications/data-sovereignty-per-microservice

Se verificó que cada servicio sea propietario de su dominio y datos, y que otro servicio acceda por API o mensajería. Respalda bases separadas, despliegue independiente y ausencia de FK físicas entre dominios. Aplicación: tres soluciones sin referencias mutuas y validaciones HTTP de IdProducto.

## [4] Transacciones

**Autor institucional:** Microsoft.

**Título:** Transactions — EF Core.

**Fecha de la página:** 19 de agosto de 2026.

**URL:** https://learn.microsoft.com/en-us/ef/core/saving/transactions

Se verificaron Default transaction behavior y Controlling transactions. Respaldan la atomicidad de un SaveChanges y las transacciones explícitas con Commit/rollback al disponerlas. Aplicación: cabecera/detalles de pedido, movimiento/stock y consumo/reserva. Los bloqueos y el comportamiento concurrente específicos del proyecto también se comprobaron empíricamente en SQL Server; esta referencia no sustituye esas pruebas.

## [5] Pruebas en Postman

**Autor institucional:** Postman.

**Título:** Test your API using the Collection Runner.

**Fecha de publicación:** no consta en el contenido recuperado; se cita como s. f.

**URL:** https://learning.postman.com/docs/collections/running-collections/intro-to-collection-runs/

Se verificaron Configure a collection run, Debug run results y Export collection runs. Respaldan la ejecución ordenada, el intercambio de variables, las aserciones y la conservación de evidencias. Aplicación: colección v2.1 entregada y guía del Runner. La automatización observada se ejecutó con Newman, sin atribuir al estudiante una ejecución manual de Postman.

## Documentos de la actividad

Instituto Superior Tecnológico Cordillera. *Aplicaciones Distribuidas — Unidad 1 — Práctica 1: Diseño de Arquitecturas Distribuidas*, AD01, 2 páginas. Y *Política de IA: Uso opcional, responsable y verificable*, 1 página. Consultados a partir del texto y las imágenes adjuntos en la conversación. No se proporcionó URL pública ni fecha editorial; no se inventan.

## Incertidumbres declaradas

- No se proporcionaron el PEA, un acceso a la Biblioteca Virtual ITSCO ni los nombres de los archivos PDF originales. No se afirma haberlos consultado.
- El usuario confirmó el nombre completo Pablo Matias Roman Ruiz, el curso 4.º B y el nombre corto Matias Roman para la entrega.
- No se verificó identidad real de clientes: todos los ejemplos y números de cédula son sintéticos.
- La figura muestra conexiones conceptuales; no especifica un protocolo de reserva distribuida ni pagos. El informe delimita el alcance implementado.
- No se usan enlaces genéricos de ChatGPT, YouTube o Stack Overflow como respaldo técnico: no se consultaron publicaciones concretas de esas plataformas en esta sesión.
