"""Prueba real contra SQL Server. Inicia y detiene únicamente sus propios procesos."""
import concurrent.futures
import json
import os
from pathlib import Path
import socket
import subprocess
import time
import urllib.error
import urllib.request
from datetime import datetime, timezone

ROOT = Path(__file__).resolve().parents[1]
ARTIFACTS = ROOT / "artifacts"
EVIDENCIAS = ROOT / "docs" / "evidencias"
procesos = {}
logs = []
resultados = []


def http(puerto, metodo, ruta, datos=None):
    req = urllib.request.Request(f"http://localhost:{puerto}{ruta}", method=metodo,
                                 data=None if datos is None else json.dumps(datos).encode(),
                                 headers={"Content-Type": "application/json"})
    try:
        with urllib.request.urlopen(req, timeout=20) as r:
            return r.status, json.loads(r.read() or "null")
    except urllib.error.HTTPError as e:
        return e.code, json.loads(e.read() or "null")


def comprobar(nombre, condicion, detalle):
    resultados.append({"prueba": nombre, "correcta": bool(condicion), "detalle": detalle})
    if not condicion:
        raise AssertionError(f"{nombre}: {detalle}")


def detener(nombre):
    p = procesos.pop(nombre, None)
    if p is not None and p.poll() is None:
        p.terminate()
        p.wait(timeout=20)


def iniciar(nombre, puerto):
    carpeta = ROOT / nombre / f"{nombre}.Api"
    dll = carpeta / "bin" / "Debug" / "net10.0" / f"{nombre}.Api.dll"
    env = dict(os.environ, ASPNETCORE_ENVIRONMENT="Development", ASPNETCORE_URLS=f"http://localhost:{puerto}")
    log = open(ARTIFACTS / f"{nombre}.log", "w", encoding="utf-8")
    logs.append(log)
    procesos[nombre] = subprocess.Popen(["dotnet", str(dll)], cwd=carpeta, env=env, stdout=log, stderr=log)
    for _ in range(100):
        if procesos[nombre].poll() is not None:
            raise RuntimeError(f"{nombre} terminó. Revisa artifacts/{nombre}.log")
        try:
            if http(puerto, "GET", "/health")[0] == 200:
                return
        except (OSError, urllib.error.URLError):
            pass
        time.sleep(0.3)
    raise RuntimeError(f"{nombre} no inició a tiempo.")


def simultaneas(metodo, ruta, cuerpo=None):
    with concurrent.futures.ThreadPoolExecutor(max_workers=2) as pool:
        futures = [pool.submit(http, 5102, metodo, ruta, cuerpo) for _ in range(2)]
        return [f.result() for f in futures]


def main():
    ARTIFACTS.mkdir(exist_ok=True)
    EVIDENCIAS.mkdir(parents=True, exist_ok=True)
    for puerto in (5101, 5102, 5103):
        with socket.socket() as s:
            if s.connect_ex(("localhost", puerto)) == 0:
                raise RuntimeError(f"Cierra la API del puerto {puerto} antes de ejecutar la verificación.")
    try:
        for nombre, puerto in [("Catalogo", 5101), ("Inventario", 5102), ("Pedidos", 5103)]:
            iniciar(nombre, puerto)
        comando = ["cmd.exe", "/c", "npx.cmd", "--yes", "newman@6.2.3", "run",
                   "postman/ComercioMultibodega.postman_collection.json",
                   "--reporters", "cli,json", "--reporter-json-export", "artifacts/newman.json",
                   "--export-collection", "artifacts/coleccion-ejecutada.json"]
        with open(ARTIFACTS / "newman.log", "w", encoding="utf-8") as log:
            ejecucion = subprocess.run(comando, cwd=ROOT, stdout=log, stderr=log, timeout=240)
        reporte = json.loads((ARTIFACTS / "newman.json").read_text(encoding="utf-8"))
        stats = reporte["run"]["stats"]
        fallas = [{"peticion": f.get("source", {}).get("name"), "error": f.get("error", {}).get("message")}
                  for f in reporte["run"]["failures"]]
        comprobar("Coleccion Postman ejecutada con Newman", ejecucion.returncode == 0 and not fallas,
                  {"estadisticas": stats, "fallas": fallas})
        # Se extraen los IDs de las respuestas reales del reporte, no del estado
        # de variables exportado por Newman (que puede conservar los originales).
        creados = {e["item"]["name"]: json.loads(bytes(e["response"]["stream"]["data"]))
                   for e in reporte["run"]["executions"]
                   if e["item"]["name"] in ("Crear producto", "Crear bodega", "Crear cliente ficticio")}
        producto = creados["Crear producto"]["idProducto"]
        bodega = creados["Crear bodega"]["idBodega"]
        cliente = creados["Crear cliente ficticio"]["idCliente"]
        reserva = {"idProducto": producto, "idBodega": bodega, "cantidad": 8}
        respuestas = simultaneas("POST", "/api/reservas", reserva)
        codigos = sorted(r[0] for r in respuestas)
        comprobar("Dos reservas simultaneas no sobrevenden", codigos == [201, 409], codigos)
        id_reserva = next(r[1]["idReserva"] for r in respuestas if r[0] == 201)
        respuestas = simultaneas("PATCH", f"/api/reservas/{id_reserva}/consumir")
        codigos = sorted(r[0] for r in respuestas)
        comprobar("Una reserva no se consume dos veces", codigos == [200, 409], codigos)
        estado = http(5102, "GET", f"/api/existencias?idProducto={producto}&idBodega={bodega}")[1][0]
        comprobar("Stock tras consumo concurrente", estado["stock"] == 4 and estado["disponible"] == 4, estado)
        codigo, b = http(5102, "POST", "/api/bodegas", {
            "nombre": f"Concurrente-{time.time_ns()}", "ciudad": "Quito", "direccion": "Laboratorio ficticio"})
        comprobar("Bodega aislada para entradas concurrentes", codigo == 201, codigo)
        entrada = {"idProducto": producto, "idBodega": b["idBodega"], "tipo": "Entrada", "cantidad": 5}
        respuestas = simultaneas("POST", "/api/movimientos", entrada)
        codigos = sorted(r[0] for r in respuestas)
        comprobar("Entradas concurrentes en pareja inicialmente vacia", codigos == [201, 201], codigos)
        estado = http(5102, "GET", f'/api/existencias?idProducto={producto}&idBodega={b["idBodega"]}')[1][0]
        comprobar("Entradas no pierden unidades", estado["stock"] == 10, estado)
        codigo, _ = http(5102, "POST", "/api/reservas", {**reserva, "cantidad": 2})
        comprobar("Reserva para cancelacion concurrente", codigo == 201, codigo)
        ultima = http(5102, "GET", "/api/reservas")[1][-1]["idReserva"]
        codigos = sorted(r[0] for r in simultaneas("PATCH", f"/api/reservas/{ultima}/cancelar"))
        comprobar("Una reserva no se cancela dos veces", codigos == [200, 409], codigos)

        # Dependencia HTTP caída: el pedido no se guarda parcialmente.
        antes = len(http(5103, "GET", "/api/pedidos")[1])
        detener("Inventario")
        codigo, _ = http(5103, "POST", "/api/pedidos", {"idCliente": cliente, "detalles": [{"idProducto": producto, "cantidad": 1}]})
        despues = len(http(5103, "GET", "/api/pedidos")[1])
        comprobar("Pedidos devuelve 503 si Inventario cae y no guarda cabecera", codigo == 503 and antes == despues, {"http": codigo, "antes": antes, "despues": despues})
        iniciar("Inventario", 5102)
        detener("Catalogo")
        codigo, _ = http(5102, "POST", "/api/movimientos", entrada)
        comprobar("Inventario devuelve 503 si Catalogo cae", codigo == 503, codigo)
        codigo, _ = http(5102, "GET", "/api/existencias")
        comprobar("Inventario mantiene consultas propias sin Catalogo", codigo == 200, codigo)
        codigo, _ = http(5103, "GET", "/api/pedidos")
        comprobar("Pedidos mantiene historial sin Catalogo", codigo == 200, codigo)
    finally:
        for nombre in list(procesos):
            detener(nombre)
        for log in logs:
            log.close()
        salida = {"fechaUtc": datetime.now(timezone.utc).isoformat(), "motor": "SQL Server real, autenticacion integrada",
                  "pruebas": resultados}
        (EVIDENCIAS / "resultado-integracion.json").write_text(json.dumps(salida, ensure_ascii=False, indent=2), encoding="utf-8")
    print(f"Verificación terminada: {len(resultados)} comprobaciones de integración correctas.")
    print(json.dumps(stats, indent=2))


if __name__ == "__main__":
    main()
