"""Genera una colección v2.1 reproducible con datos ficticios y aserciones de negocio."""
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
items = []


def request(nombre, servicio, metodo, ruta, codigo=200, cuerpo=None, captura=None, checks=()):
    tests = [f'pm.test("HTTP {codigo}", () => pm.response.to.have.status({codigo}));']
    if captura:
        variable, campo = captura
        tests.append(f'if (pm.response.code === {codigo}) pm.collectionVariables.set("{variable}", pm.response.json().{campo});')
    tests.extend(checks)
    req = {"method": metodo, "header": [], "url": "{{" + servicio + "}}" + ruta}
    if cuerpo is not None:
        req["header"].append({"key": "Content-Type", "value": "application/json"})
        req["body"] = {"mode": "raw", "raw": json.dumps(cuerpo, ensure_ascii=False), "options": {"raw": {"language": "json"}}}
        # Los identificadores deben enviarse como números JSON.
        for variable in ["categoria", "marca", "producto", "bodega", "cliente"]:
            req["body"]["raw"] = req["body"]["raw"].replace('"{{' + variable + '}}"', '{{' + variable + '}}')
    items.append({"name": nombre, "request": req, "event": [{"listen": "test", "script": {"type": "text/javascript", "exec": tests}}]})


for servicio in ["catalogo", "inventario", "pedidos"]:
    request(f"Salud de {servicio}", servicio, "GET", "/health")
items[0]["event"].append({"listen": "prerequest", "script": {"type": "text/javascript", "exec": [
    'pm.collectionVariables.set("sufijo", Date.now().toString());',
    'pm.collectionVariables.set("cedula", Date.now().toString().slice(-10));'
]}})

categoria = {"nombre": "Perifericos-{{sufijo}}", "descripcion": "Datos ficticios para la practica"}
marca = {"nombre": "MarcaDemo-{{sufijo}}", "paisOrigen": "Ecuador"}
producto = {"idCategoria": "{{categoria}}", "idMarca": "{{marca}}", "nombre": "Teclado de prueba", "sku": "TEC-{{sufijo}}", "precio": 25.50, "estado": True}
bodega = {"nombre": "BodegaDemo-{{sufijo}}", "ciudad": "Quito", "direccion": "Direccion ficticia de laboratorio"}
cliente = {"nombres": "Cliente Ficticio de Prueba", "cedula": "{{cedula}}", "correo": "demo-{{sufijo}}@example.com"}
movimiento = {"idProducto": "{{producto}}", "idBodega": "{{bodega}}", "tipo": "Entrada", "cantidad": 20}
reserva = {"idProducto": "{{producto}}", "idBodega": "{{bodega}}", "cantidad": 5}
pedido = {"idCliente": "{{cliente}}", "detalles": [{"idProducto": "{{producto}}", "cantidad": 2}]}

request("Crear categoria", "catalogo", "POST", "/api/categorias", 201, categoria, ("categoria", "idCategoria"))
request("Crear marca", "catalogo", "POST", "/api/marcas", 201, marca, ("marca", "idMarca"))
request("Crear producto", "catalogo", "POST", "/api/productos", 201, producto, ("producto", "idProducto"), ['pm.test("Location de recurso", () => pm.expect(pm.response.headers.get("Location")).to.include("/api/productos/"));'])
for ruta, variable in [("categorias", "categoria"), ("marcas", "marca"), ("productos", "producto")]:
    request(f"Listar {ruta}", "catalogo", "GET", f"/api/{ruta}")
    request(f"Obtener {variable}", "catalogo", "GET", f"/api/{ruta}/{{{{{variable}}}}}")
request("Actualizar categoria", "catalogo", "PUT", "/api/categorias/{{categoria}}", 200, categoria)
request("Actualizar marca", "catalogo", "PUT", "/api/marcas/{{marca}}", 200, marca)
request("Actualizar producto", "catalogo", "PUT", "/api/productos/{{producto}}", 200, producto)
request("Rechazar SKU duplicado", "catalogo", "POST", "/api/productos", 409, producto)
request("Rechazar precio negativo", "catalogo", "POST", "/api/productos", 400, {**producto, "precio": -1})
request("Rechazar tres decimales", "catalogo", "POST", "/api/productos", 400, {**producto, "precio": 1.123})
request("Rechazar FK inexistente", "catalogo", "POST", "/api/productos", 400, {**producto, "idCategoria": 2147483647})
request("Rechazar nombre vacio", "catalogo", "POST", "/api/categorias", 400, {**categoria, "nombre": " "})
request("Producto inexistente 404", "catalogo", "GET", "/api/productos/2147483647", 404)
request("Proteger categoria relacionada", "catalogo", "DELETE", "/api/categorias/{{categoria}}", 409)
request("Proteger marca relacionada", "catalogo", "DELETE", "/api/marcas/{{marca}}", 409)

request("Crear bodega", "inventario", "POST", "/api/bodegas", 201, bodega, ("bodega", "idBodega"))
request("Listar bodegas", "inventario", "GET", "/api/bodegas")
request("Obtener bodega", "inventario", "GET", "/api/bodegas/{{bodega}}")
request("Actualizar bodega", "inventario", "PUT", "/api/bodegas/{{bodega}}", 200, bodega)
request("Registrar entrada 20", "inventario", "POST", "/api/movimientos", 201, movimiento, ("movimiento", "idMovimiento"))
request("Movimiento guardado", "inventario", "GET", "/api/movimientos/{{movimiento}}")
request("Listar movimientos", "inventario", "GET", "/api/movimientos")
request("Rechazar cantidad cero", "inventario", "POST", "/api/movimientos", 400, {**movimiento, "cantidad": 0})
request("Rechazar tipo desconocido", "inventario", "POST", "/api/movimientos", 400, {**movimiento, "tipo": "Ajuste"})
request("Rechazar producto remoto inexistente", "inventario", "POST", "/api/movimientos", 400, {**movimiento, "idProducto": 2147483647})
request("Rechazar bodega inexistente", "inventario", "POST", "/api/movimientos", 400, {**movimiento, "idBodega": 2147483647})
request("Reservar 5", "inventario", "POST", "/api/reservas", 201, reserva, ("reserva", "idReserva"))
request("Ver reserva", "inventario", "GET", "/api/reservas/{{reserva}}")
request("Listar reservas", "inventario", "GET", "/api/reservas")
request("Ver stock 20 y disponible 15", "inventario", "GET", "/api/existencias?idProducto={{producto}}&idBodega={{bodega}}", checks=[
    'pm.test("Stock fisico y reservado", () => { const e=pm.response.json()[0]; pm.expect(e.stock).to.eql(20); pm.expect(e.reservado).to.eql(5); pm.expect(e.disponible).to.eql(15); });',
    'pm.collectionVariables.set("existencia", pm.response.json()[0].idExistencia);'
])
request("Ver existencia por id", "inventario", "GET", "/api/existencias/{{existencia}}")
request("Rechazar reserva excesiva", "inventario", "POST", "/api/reservas", 409, {**reserva, "cantidad": 16})
request("Rechazar salida que invade reserva", "inventario", "POST", "/api/movimientos", 409, {**movimiento, "tipo": "Salida", "cantidad": 16})
request("Cancelar reserva", "inventario", "PATCH", "/api/reservas/{{reserva}}/cancelar", checks=['pm.test("Reserva cancelada", () => pm.expect(pm.response.json().estado).to.eql("Cancelada"));'])
request("No cerrar reserva dos veces", "inventario", "PATCH", "/api/reservas/{{reserva}}/consumir", 409)
request("Crear nueva reserva", "inventario", "POST", "/api/reservas", 201, reserva, ("reserva", "idReserva"))
request("Consumir reserva y generar salida", "inventario", "PATCH", "/api/reservas/{{reserva}}/consumir", checks=['pm.test("Reserva consumida", () => pm.expect(pm.response.json().estado).to.eql("Consumida"));'])
request("Salida libre de 3", "inventario", "POST", "/api/movimientos", 201, {**movimiento, "tipo": "Salida", "cantidad": 3})
request("Stock final 12 sin reserva activa", "inventario", "GET", "/api/existencias?idProducto={{producto}}&idBodega={{bodega}}", checks=[
    'pm.test("Balance de stock", () => {const e=pm.response.json()[0]; pm.expect(e.stock).to.eql(12); pm.expect(e.disponible).to.eql(12); pm.expect(e.reservado).to.eql(0);});'
])
request("Proteger bodega con historial", "inventario", "DELETE", "/api/bodegas/{{bodega}}", 409)
request("Reserva inexistente 404", "inventario", "GET", "/api/reservas/2147483647", 404)

request("Crear cliente ficticio", "pedidos", "POST", "/api/clientes", 201, cliente, ("cliente", "idCliente"))
request("Listar clientes", "pedidos", "GET", "/api/clientes")
request("Obtener cliente", "pedidos", "GET", "/api/clientes/{{cliente}}")
request("Actualizar cliente", "pedidos", "PUT", "/api/clientes/{{cliente}}", 200, cliente)
request("Rechazar cedula repetida", "pedidos", "POST", "/api/clientes", 409, cliente)
request("Rechazar correo invalido", "pedidos", "POST", "/api/clientes", 400, {**cliente, "correo": "no-es-correo"})
request("Rechazar cedula corta", "pedidos", "POST", "/api/clientes", 400, {**cliente, "cedula": "123"})
request("Crear pedido con precios del catalogo", "pedidos", "POST", "/api/pedidos", 201, pedido, ("pedido", "idPedido"), [
    'pm.test("Total calculado 51", () => {const p=pm.response.json(); pm.expect(p.total).to.eql(51); pm.expect(p.detalles[0].precioUnitario).to.eql(25.5); pm.expect(p.estado).to.eql("Pendiente");});'
])
request("Listar pedidos", "pedidos", "GET", "/api/pedidos")
request("Obtener pedido", "pedidos", "GET", "/api/pedidos/{{pedido}}")
request("Obtener detalles", "pedidos", "GET", "/api/pedidos/{{pedido}}/detalles")
request("Rechazar pedido sin detalles", "pedidos", "POST", "/api/pedidos", 400, {**pedido, "detalles": []})
request("Rechazar detalle nulo", "pedidos", "POST", "/api/pedidos", 400, {**pedido, "detalles": [None]})
request("Rechazar cantidad negativa en detalle", "pedidos", "POST", "/api/pedidos", 400, {**pedido, "detalles": [{"idProducto": "{{producto}}", "cantidad": -1}]})
request("Rechazar detalles repetidos", "pedidos", "POST", "/api/pedidos", 400, {**pedido, "detalles": pedido["detalles"] * 2})
request("Rechazar cliente inexistente", "pedidos", "POST", "/api/pedidos", 400, {**pedido, "idCliente": 2147483647})
request("Rechazar pedido sin stock", "pedidos", "POST", "/api/pedidos", 409, {**pedido, "detalles": [{"idProducto": "{{producto}}", "cantidad": 100}]})
request("Actualizar pedido pendiente", "pedidos", "PUT", "/api/pedidos/{{pedido}}", 200, {**pedido, "detalles": [{"idProducto": "{{producto}}", "cantidad": 3}]}, checks=[
    'pm.test("Total recalculado 76.50", () => pm.expect(pm.response.json().total).to.eql(76.5));'
])
request("Confirmacion administrativa", "pedidos", "PATCH", "/api/pedidos/{{pedido}}/confirmar", 204)
request("No editar pedido confirmado", "pedidos", "PUT", "/api/pedidos/{{pedido}}", 409, pedido)
request("Proteger cliente con pedidos", "pedidos", "DELETE", "/api/clientes/{{cliente}}", 409)
request("Crear pedido para cancelar", "pedidos", "POST", "/api/pedidos", 201, pedido, ("pedidoCancelar", "idPedido"))
request("Cancelar conservando historial", "pedidos", "DELETE", "/api/pedidos/{{pedidoCancelar}}", 204)
request("Comprobar cancelacion", "pedidos", "GET", "/api/pedidos/{{pedidoCancelar}}", checks=['pm.test("Estado Cancelado", () => pm.expect(pm.response.json().estado).to.eql("Cancelado"));'])
request("Pedido inexistente 404", "pedidos", "GET", "/api/pedidos/2147483647", 404)
request("Desactivar producto sin borrar historial", "catalogo", "DELETE", "/api/productos/{{producto}}", 204)
request("Rechazar movimiento de inactivo", "inventario", "POST", "/api/movimientos", 400, movimiento)
request("Rechazar pedido de inactivo", "pedidos", "POST", "/api/pedidos", 400, pedido)
request("Reactivar producto de demostracion", "catalogo", "PUT", "/api/productos/{{producto}}", 200, producto)

# Verificación de DELETE físico para maestros sin relaciones.
for servicio, ruta, cuerpo, campo in [
    ("catalogo", "categorias", {"nombre": "Temporal-{{sufijo}}", "descripcion": "Temporal"}, "idCategoria"),
    ("catalogo", "marcas", {"nombre": "Temporal-{{sufijo}}", "paisOrigen": "Ecuador"}, "idMarca"),
    ("inventario", "bodegas", {**bodega, "nombre": "Temporal-{{sufijo}}"}, "idBodega"),
    ("pedidos", "clientes", {**cliente, "cedula": "000{{cedula}}", "correo": "temporal-{{sufijo}}@example.com"}, "idCliente")
]:
    if ruta == "clientes":
        cuerpo["cedula"] = "{{cedulaTemporal}}"
        items[0]["event"][1]["script"]["exec"].append('pm.collectionVariables.set("cedulaTemporal", (Number(pm.collectionVariables.get("cedula"))+1).toString().padStart(10,"0").slice(-10));')
    request(f"Crear temporal {ruta}", servicio, "POST", "/api/" + ruta, 201, cuerpo, ("temporal", campo))
    request(f"Eliminar temporal {ruta}", servicio, "DELETE", "/api/" + ruta + "/{{temporal}}", 204)
    request(f"Temporal eliminado {ruta}", servicio, "GET", "/api/" + ruta + "/{{temporal}}", 404)

coleccion = {
    "info": {"name": "AD01 - Comercio multibodega - Matias Roman - 4B", "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json", "description": "Estudiante: Pablo Matias Roman Ruiz. Curso: 4.º B. Ejecutar completa y en orden. Genera datos ficticios únicos y conserva un ejemplo para sustentar. Confirmar un pedido es administrativo; las reservas y salidas se gestionan explícitamente en Inventario."},
    "variable": [{"key": k, "value": v} for k, v in {"catalogo": "http://localhost:5101", "inventario": "http://localhost:5102", "pedidos": "http://localhost:5103"}.items()],
    "item": items
}
if __name__ == "__main__":
    destino = ROOT / "postman"
    destino.mkdir(exist_ok=True)
    (destino / "ComercioMultibodega.postman_collection.json").write_text(json.dumps(coleccion, ensure_ascii=False, indent=2), encoding="utf-8")
    print(f"Colección creada: {len(items)} peticiones.")
