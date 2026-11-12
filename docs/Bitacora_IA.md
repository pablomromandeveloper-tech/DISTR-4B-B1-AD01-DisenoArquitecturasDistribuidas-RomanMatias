# Bitácora de uso de IA

## Identificación

- Actividad: AD01 — Diseño de Arquitecturas Distribuidas.
- Estudiante confirmado por el usuario: Pablo Matias Roman Ruiz.
- Nivel/paralelo: 4.º B. Nombre corto de entrega: Matias Roman.
- Herramienta: OpenCode, asistente de programación.
- Modelo informado por el entorno: OpenAI `gpt-6.1-sol`.
- Fecha de esta sesión: 11/11/2026.
- Alcance: implementación asistida de código, configuración, documentación, generación de datos ficticios y pruebas automáticas.

## Consulta significativa 1 — prompt literal del usuario

```text
cd C:\Users\MSI\source\repos ||ahi en esa carpeta quiero que crees proyectos separados SOA (Arquitectura Orientada a Servicios), o api en si en diferentes proyectos diferentes en vs y deben ser proyectos diferentes o soluciones diferentes segun los documentos, preparame todo bien como echo por estudiante y prepara todo segun esto: C:\Users\MSI\Downloads\Plantilla_Registro_Prompts_y_Fuentes.docx||[PDF 1]|| [PDF 2]
```

**Material adicional recibido:** el usuario incorporó el texto de la política de IA y las dos páginas de la práctica AD01, con imágenes de la arquitectura y el formato de GitHub. Se emplearon como requisitos de esta misma consulta. No se atribuyen prompts adicionales al estudiante.

**Síntesis de la respuesta:** creación de tres soluciones independientes ASP.NET Core, tres bases SQL Server y sus migraciones, controladores REST, validaciones, transacciones de inventario, llamadas HTTP, colección de Postman, documentación y declaración de IA.

**Aprovechado:** separación por dominio; tablas y campos mínimos; FK físicas locales y referencias lógicas; cálculo de totales en el servidor; precios históricos; stock controlado por movimientos; reservas con estados; pruebas positivas, negativas, concurrentes y de dependencia caída.

**Descartado o corregido durante la sesión:**

- Se corrigió la ubicación de los atributos de validación en DTO record: MVC requiere metadata en los parámetros del constructor, no en las propiedades generadas.
- Se corrigió la interpretación del límite decimal de Range bajo la configuración regional de Windows mediante `ParseLimitsInInvariantCulture`.
- Se actualizaron dependencias iniciales a EF Core 10.0.15 y Swashbuckle 10.3.1 después de observar advertencias de paquetes antiguos. La compilación final quedó sin advertencias.
- Se corrigió el verificador para extraer IDs de las respuestas reales de Newman; no se supuso que la exportación conservaría variables dinámicas.
- No se adoptaron las referencias genéricas ni la consulta del 10/11/2026 que ya contenía la plantilla como si se hubieran realizado en esta sesión. La plantilla original no se modificó.
- No se agregó una transacción compartida entre bases ni una reserva automática de pedido que la práctica no define. La disponibilidad de pedido es una consulta puntual y se documenta esa limitación.

## Consulta significativa 2 — identificación y entrega

El usuario solicitó corregir la identificación en toda la entrega y publicar el proyecto en su cuenta de GitHub. Fragmentos literales relevantes de su mensaje:

```text
Pablo Matias Roman Ruiz 4to b
```

```text
Matias Roman
```

**Aplicación:** nombre completo en portada, declaración y metadatos; 4.º B como curso; Matias Roman como nombre corto en archivos y colección; `RomanMatias` en el repositorio según el formato académico. Los resultados de pruebas conservan sus fechas reales. El usuario autorizó el inicio de sesión oficial de GitHub CLI y confirmó la publicación en su cuenta autenticada `pablomromandeveloper-tech`.

**Repositorio de entrega:** https://github.com/pablomromandeveloper-tech/DISTR-4B-B1-AD01-DisenoArquitecturasDistribuidas-RomanMatias

## Fuentes y comprobación

El prompt original no incluía la petición de fuentes oficiales que exige la política. Para dar trazabilidad se consultaron directamente cinco documentos oficiales y se declararon incertidumbres en `Fuentes_Verificadas.md`. No se alteró retrospectivamente el texto del prompt para aparentar que contenía esa petición.

Para futuras consultas, el estudiante debe añadir una instrucción como la siguiente (**recomendada, no ejecutada como un prompt adicional en esta sesión**):

> Incluye libros o fuentes oficiales verificables con autor, título, fecha y URL que respalden cada explicación técnica. Declara cualquier incertidumbre y no inventes referencias.

## Verificación realizada por el asistente

- Compilación real de tres soluciones .NET 10.
- Creación y migración de CatalogoDB, InventarioDB y PedidosDB en SQL Server local.
- Ejecución automatizada de la colección Postman mediante Newman: 90 peticiones, 98 aserciones, cero fallas en la ejecución final.
- Concurrencia: una sola reserva de ocho unidades aceptada sobre doce disponibles; un solo consumo de la misma reserva; entradas simultáneas sin pérdida de unidades; una sola cancelación de reserva.
- Caídas: HTTP 503 cuando una escritura necesita un servicio no disponible, sin guardar un pedido parcial; conservación de consultas locales.
- Evidencia concreta: `docs/evidencias/resultado-integracion.json`. Estos resultados corresponden al asistente, no a una ejecución personal ya realizada por el estudiante.

## Revisión y comprensión del estudiante — por completar personalmente

Fecha de revisión: ____________________

Cambios propios realizados y su motivo: ________________________________________

Fuentes abiertas y comprobadas personalmente: __________________________________

Resultado de mi ejecución de Postman: _________________________________________

Decisiones que puedo explicar: ________________________________________________

Firma del estudiante: ____________________
