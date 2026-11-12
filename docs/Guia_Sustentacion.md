# Guía de sustentación — AD01

## Recorrido de demostración (8–10 minutos)

1. Abre las tres soluciones y muestra sus cadenas de conexión distintas.
2. En SSMS muestra las tres bases, sus tablas y las siete FK físicas locales.
3. Explica los límites: Catálogo administra datos comerciales, Inventario unidades físicas y Pedidos órdenes/clientes.
4. Ejecuta las APIs; abre las tres interfaces Swagger.
5. Importa y ejecuta la colección Postman completa.
6. Muestra la entrada de 20 unidades, reserva de 5, disponible de 15, cancelación, consumo de otra reserva y salida de 3: quedan 12.
7. Muestra el pedido: 2 × 25,50 = 51,00. Al editarlo a 3 unidades, Total = 76,50. El cliente no envía precio ni total.
8. Presenta un error 400 (cantidad cero), 404 (ID inexistente) y 409 (salida que invade una reserva).
9. Explica que confirmar pedido es administrativo y no reserva stock automáticamente. Enseña el flujo explícito de Inventario y la limitación de la consulta puntual de disponibilidad.
10. Presenta el registro de IA y distingue lo generado, lo corregido, lo probado por el asistente y tus modificaciones personales.

## Preguntas probables

### ¿Por qué tres soluciones y tres bases?

Cada servicio agrupa entidades con una responsabilidad de negocio. Se compila y ejecuta por separado. Separar bases evita que otro servicio dependa de tablas internas y permite cambios independientes [3].

### ¿Esto es SOA o microservicios?

Comparte principios de orientación a servicios, pero el enunciado pide concretamente tres microservicios REST. Se implementan como tres aplicaciones ASP.NET Core separadas, cada una propietaria de sus datos.

### ¿Qué diferencia hay entre FK física y lógica?

La FK física es una restricción SQL dentro de la misma base; por ejemplo Producto.IdMarca. Una referencia lógica almacena un ID de otro dominio y se comprueba mediante su API; por ejemplo Existencia.IdProducto. No se crean joins ni FK entre las tres bases.

### ¿Por qué DELETE de producto no borra la fila?

Otros dominios conservan referencias e historial. Desactivar evita nuevos movimientos/pedidos y mantiene el significado de datos anteriores. Los maestros sin relaciones sí pueden borrarse físicamente.

### ¿Stock y disponible son iguales?

Solo cuando no existen reservas activas. Stock representa unidades físicas; disponible descuenta lo comprometido. Reservar no mueve mercancía. Consumir una reserva sí registra salida y reduce stock.

### ¿Por qué no existe PUT libre de Existencia o Movimiento?

Modificar una cifra sin un movimiento haría imposible explicar el saldo. Los movimientos quedan como historial inmutable; las correcciones se hacen con movimientos compensatorios explícitos.

### ¿Cómo evitas dos reservas sobre el mismo stock?

StockService inicia una transacción Serializable y toma `UPDLOCK, HOLDLOCK` sobre la pareja producto/bodega antes de comprobar y cambiar el saldo. Las dos solicitudes se serializan. Se probó con SQL Server real: una recibió 201 y la otra 409.

### ¿Qué asegura una transacción?

Que stock y movimiento, o reserva consumida y salida, se confirmen juntos. Un error revierte todos esos cambios locales. No se confunde esa transacción con una transacción distribuida entre servicios [4].

### ¿Por qué el precio está en DetallePedido?

Es una copia histórica obtenida del Catálogo al registrar/editar el pedido pendiente. Si el precio comercial cambia después, el pedido guardado no cambia. Total se calcula sumando cantidad por precio unitario.

### ¿Un pedido confirmado asegura mercancía?

No en este alcance. Es confirmación administrativa. La asignación requiere crear y consumir reservas explícitas en Inventario. Para automatizar un proceso completo harían falta asociaciones pedido-reserva, idempotencia y compensaciones ante fallas entre servicios. No se afirma haber implementado esa ampliación.

### ¿Qué ocurre si cae un servicio?

Las consultas sobre datos locales siguen funcionando. Una operación que necesita validar contra el servicio caído recibe 503, y un pedido no guarda su cabecera sin validación completa. Se probaron las dos situaciones.

### ¿Qué valida la cédula?

Diez dígitos y unicidad. No verifica identidad ni algoritmo oficial. Los datos de pruebas son ficticios; no se consultó una entidad externa.

### ¿Qué parte hizo la IA?

Se documenta la asistencia en implementación, documentos y pruebas. El estudiante debe explicar y demostrar lo que revisó y cambió personalmente. La declaración tiene espacios para completar esa evidencia, sin atribuirle de antemano la revisión.

## Código que conviene estudiar

- `Catalogo.Api/Controllers/ProductosController.cs`: FK, precio y baja lógica.
- `Inventario.Api/Services/StockService.cs`: transacciones, disponibilidad y reservas.
- `Pedidos.Api/Services/PedidoService.cs`: integración HTTP, precios y validación.
- `Pedidos.Api/Controllers/PedidosController.cs`: total, edición atómica y estados.
- `Data/*Context.cs`: índices, relaciones y restricciones SQL.
- `Contracts/Solicitudes.cs`: DataAnnotations y DTO sin sobreexponer entidades.

Referencias [1–5]: `Fuentes_Verificadas.md`.
