# Evidencias reproducibles

`resultado-integracion.json` se genera al ejecutar `Verificar.ps1`. Contiene las estadísticas reales de la colección Postman ejecutada mediante Newman y las comprobaciones adicionales de concurrencia y caída de servicios.

El informe completo de Newman y las respuestas se guardan localmente en `artifacts/newman.json`; el registro de consola está en `artifacts/newman.log`. Los logs completos no se versionan.

Los datos utilizados son ficticios. Cada ejecución genera nombres, SKU y correos diferentes. Las cédulas son cadenas numéricas sintéticas de diez dígitos: se valida formato y unicidad, no identidad ni dígito verificador del Registro Civil.
