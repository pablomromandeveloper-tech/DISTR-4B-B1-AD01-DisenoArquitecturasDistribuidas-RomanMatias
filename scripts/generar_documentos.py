"""Genera documentos derivados. No altera la plantilla original del estudiante.

Requisitos: py -m pip install python-docx reportlab
Uso: py scripts/generar_documentos.py [--plantilla ruta.docx]
"""
import argparse
from html import escape
from pathlib import Path
import re

from docx import Document
from docx.shared import Cm, Pt
from reportlab.graphics.shapes import Drawing, Rect, String, Line, Polygon
from reportlab.graphics import renderSVG
from reportlab.lib import colors
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.pagesizes import A4
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle

ROOT = Path(__file__).resolve().parents[1]
DOCS = ROOT / "docs"
FONT = Path("C:/Windows/Fonts/arial.ttf")
if FONT.exists():
    pdfmetrics.registerFont(TTFont("Texto", str(FONT)))
    pdfmetrics.registerFont(TTFont("TextoNegrita", "C:/Windows/Fonts/arialbd.ttf"))
    pdfmetrics.registerFontFamily("Texto", normal="Texto", bold="TextoNegrita", italic="Texto", boldItalic="TextoNegrita")
else:
    pdfmetrics.registerFontFamily("Texto", normal="Helvetica", bold="Helvetica-Bold")

NORMAL = "Texto" if FONT.exists() else "Helvetica"
BOLD = "TextoNegrita" if FONT.exists() else "Helvetica-Bold"
styles = getSampleStyleSheet()
styles.add(ParagraphStyle("Cuerpo", fontName=NORMAL, fontSize=10, leading=14, spaceAfter=7, splitLongWords=True))
styles.add(ParagraphStyle("TituloDoc", parent=styles["Cuerpo"], fontName=BOLD, fontSize=21, leading=26, textColor=colors.HexColor("#163B5C"), spaceAfter=15))
styles.add(ParagraphStyle("SeccionDoc", parent=styles["Cuerpo"], fontName=BOLD, fontSize=14, leading=19, spaceBefore=11, spaceAfter=8, keepWithNext=True, textColor=colors.HexColor("#163B5C")))
styles.add(ParagraphStyle("SubseccionDoc", parent=styles["SeccionDoc"], fontSize=11, leading=15))
styles.add(ParagraphStyle("CeldaDoc", parent=styles["Cuerpo"], fontSize=8, leading=11, spaceAfter=0))
styles.add(ParagraphStyle("CodigoDoc", parent=styles["Cuerpo"], fontName="Courier", fontSize=8, leading=11, backColor=colors.HexColor("#F2F4F7")))
styles.add(ParagraphStyle("RegistroCuerpo", parent=styles["Cuerpo"], fontSize=9.5, leading=12.5, spaceAfter=5))
styles.add(ParagraphStyle("RegistroCampo", parent=styles["SubseccionDoc"], fontSize=10.5, leading=14, spaceBefore=7, spaceAfter=4))


def formato(texto):
    texto = escape(texto)
    texto = re.sub(r"\*\*(.*?)\*\*", r"<b>\1</b>", texto)
    texto = re.sub(r"`([^`]+)`", r"\1", texto)
    texto = re.sub(r"https?://[^\s<]+", lambda m: '<link href="' + m.group(0) + '" color="#175284">' + m.group(0) + '</link>', texto)
    return texto


def p(texto, estilo="Cuerpo"):
    return Paragraph(formato(texto).replace("\n", "<br/>"), styles[estilo])


def pie(canvas, doc):
    canvas.setStrokeColor(colors.HexColor("#D6DFE7"))
    canvas.line(43, 37, A4[0] - 43, 37)
    canvas.setFont(NORMAL, 8)
    canvas.setFillColor(colors.HexColor("#52616D"))
    canvas.drawString(43, 24, "AD01 | Matias Roman | 4.º B | 11/11/2026")
    canvas.drawRightString(A4[0] - 43, 24, f"Página {doc.page}")


def pdf(destino, elementos, titulo):
    SimpleDocTemplate(str(destino), pagesize=A4, rightMargin=43, leftMargin=43,
                      topMargin=43, bottomMargin=51, title=titulo, author="Pablo Matias Roman Ruiz · 4.º B · documento asistido con OpenCode").build(
                          elementos, onFirstPage=pie, onLaterPages=pie)


def arquitectura():
    d = Drawing(508, 245)
    def caja(x, y, w, h, titulo, subtitulo, color):
        d.add(Rect(x, y, w, h, rx=8, ry=8, fillColor=colors.HexColor(color), strokeColor=colors.HexColor("#466479")))
        d.add(String(x + w / 2, y + h - 20, titulo, fontName=BOLD, fontSize=11, textAnchor="middle"))
        if subtitulo:
            d.add(String(x + w / 2, y + 12, subtitulo, fontName=NORMAL, fontSize=8, textAnchor="middle"))
    def flecha(x1, y1, x2, y2):
        d.add(Line(x1, y1, x2, y2, strokeColor=colors.HexColor("#466479")))
        dx, dy = x2 - x1, y2 - y1
        length = (dx * dx + dy * dy) ** 0.5
        ux, uy = dx / length, dy / length
        d.add(Polygon([x2, y2, x2 - ux * 7 - uy * 3, y2 - uy * 7 + ux * 3,
                       x2 - ux * 7 + uy * 3, y2 - uy * 7 - ux * 3], fillColor=colors.HexColor("#466479")))
    caja(164, 195, 180, 40, "Cliente / Postman", "", "#F1F4F8")
    caja(164, 122, 180, 51, "Pedidos.Api :5103", "Cliente · Pedido · Detalle", "#EEE8FD")
    caja(2, 45, 155, 51, "Catalogo.Api :5101", "Categoria · Marca · Producto", "#E5EEFF")
    caja(351, 45, 155, 51, "Inventario.Api :5102", "Bodega · Stock · Mov. · Reserva", "#E0F4EF")
    caja(164, 55, 180, 35, "PedidosDB", "", "#F1F4F8")
    caja(2, 0, 155, 30, "CatalogoDB", "", "#F1F4F8")
    caja(351, 0, 155, 30, "InventarioDB", "", "#F1F4F8")
    flecha(254, 195, 254, 173)
    flecha(190, 122, 125, 96)
    flecha(318, 122, 398, 96)
    flecha(254, 122, 254, 90)
    flecha(80, 45, 80, 30)
    flecha(429, 45, 429, 30)
    flecha(351, 108, 157, 108)
    d.add(String(254, 101, "Inventario valida productos por HTTP", fontName=NORMAL, fontSize=7, textAnchor="middle"))
    return d


def markdown_pdf(origen, destino):
    lines = origen.read_text(encoding="utf-8").splitlines()
    elementos, buffer = [], []
    def flush():
        if buffer:
            elementos.append(p(" ".join(buffer)))
            buffer.clear()
    i = 0
    while i < len(lines):
        line = lines[i]
        if line.startswith("!["):
            flush()
            elementos.append(arquitectura())
            elementos.append(Spacer(1, 10))
        elif line.startswith("#"):
            flush()
            nivel = len(line) - len(line.lstrip("#"))
            estilo = "TituloDoc" if nivel == 1 else "SeccionDoc" if nivel == 2 else "SubseccionDoc"
            elementos.append(p(line.lstrip("# "), estilo))
        elif line.startswith("|"):
            flush()
            rows = []
            while i < len(lines) and lines[i].startswith("|"):
                valores = [x.strip() for x in lines[i].strip("|").split("|")]
                if not all(re.fullmatch(r"[:\- ]+", x) for x in valores):
                    rows.append([p(x, "CeldaDoc") for x in valores])
                i += 1
            table = Table(rows, colWidths=[115, 393] if len(rows[0]) == 2 else None, repeatRows=1, hAlign="LEFT")
            table.setStyle(TableStyle([
                ("BACKGROUND", (0, 0), (-1, 0), colors.HexColor("#E5EDF5")),
                ("VALIGN", (0, 0), (-1, -1), "TOP"), ("GRID", (0, 0), (-1, -1), 0.4, colors.HexColor("#CDD5DE")),
                ("LEFTPADDING", (0, 0), (-1, -1), 7), ("RIGHTPADDING", (0, 0), (-1, -1), 7),
                ("TOPPADDING", (0, 0), (-1, -1), 6), ("BOTTOMPADDING", (0, 0), (-1, -1), 6)
            ]))
            elementos += [table, Spacer(1, 9)]
            continue
        elif line.startswith("- "):
            flush()
            elementos.append(p("• " + line[2:]))
        elif not line.strip():
            flush()
        else:
            buffer.append(line)
        i += 1
    flush()
    pdf(destino, elementos, "Diseño de Arquitecturas Distribuidas — AD01")


def registro(plantilla):
    doc = Document(plantilla)
    doc.core_properties.author = "Pablo Matias Roman Ruiz"
    doc.core_properties.last_modified_by = "Matias Roman"
    doc.core_properties.title = "AD01 — Registro de fuentes y uso responsable de IA — 4.º B"
    doc.core_properties.subject = "Aplicaciones Distribuidas — Pablo Matias Roman Ruiz"
    pars = doc.paragraphs
    pars[2].text = "Actividad / Código: AD01 — Diseño de Arquitecturas Distribuidas\nEstudiante: Pablo Matias Roman Ruiz\nNivel/paralelo: 4.º B · Nombre corto: Matias Roman"
    pars[4].text = "[ ] No utilicé herramientas de IA generativa."
    pars[5].text = "[X] Utilicé IA generativa como apoyo. Este registro describe la asistencia y sus pruebas; la revisión y comprensión personal del estudiante están pendientes de completar y firmar."
    pars[6].text = "Registro de la consulta relevante de esta sesión. La plantilla original se conserva sin modificaciones."
    bitacora = (DOCS / "Bitacora_IA.md").read_text(encoding="utf-8")
    prompt = bitacora.split("```text\n", 1)[1].split("\n```", 1)[0]
    fuentes = [
        ("Microsoft", "Create web APIs with ASP.NET Core", "2026-05-06", "https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-10.0"),
        ("Microsoft", "Microsoft SQL Server Database Provider — EF Core", "2026-06-09", "https://learn.microsoft.com/en-us/ef/core/providers/sql-server/"),
        ("Microsoft", "Data sovereignty per microservice", "2018-09-20; actualización 2022", "https://learn.microsoft.com/en-us/dotnet/architecture/microservices/architect-microservice-container-applications/data-sovereignty-per-microservice"),
        ("Microsoft", "Transactions — EF Core", "2026-08-19", "https://learn.microsoft.com/en-us/ef/core/saving/transactions"),
        ("Postman", "Test your API using the Collection Runner", "s. f.", "https://learning.postman.com/docs/collections/running-collections/intro-to-collection-runs/")
    ]
    referencias = "\n\n".join(f"[{i}] {a}. ({fecha}). {titulo}.\n{url}" for i, (a, titulo, fecha, url) in enumerate(fuentes, 1))
    filas = [
        ("Herramienta / modelo", "OpenCode. Modelo informado por el entorno: OpenAI gpt-6.1-sol."),
        ("Fecha", "11/11/2026. No se presenta la consulta previa de la plantilla como una consulta de esta sesión."),
        ("Propósito de la consulta", "Preparar tres soluciones independientes de Web API con SQL Server, validaciones, pruebas Postman, documentación y registro transparente del apoyo de IA."),
        ("Prompt exacto", prompt + "\n\nEl usuario adjuntó además el texto de la política de IA y la práctica AD01 de dos páginas, con sus imágenes."),
        ("Respuesta aprovechada", "Código de los tres dominios, migraciones, reglas de stock y reservas, integración HTTP, cálculo de total, colección Postman y guías. Se corrigieron errores detectados y se probaron las tres APIs con SQL Server real."),
        ("Fuentes sugeridas por IA (al menos 3)", "Se priorizaron las cinco fuentes oficiales citadas abajo. No se usaron referencias genéricas de ChatGPT, YouTube o Stack Overflow como evidencia.\n" + "\n".join(f"[{i}] {x[1]} — {x[3]}" for i, x in enumerate(fuentes[:3], 1))),
        ("Fuentes verificadas y citadas (al menos 3)", "Las cinco fuentes fueron abiertas por el asistente el 11/11/2026. Respaldan controladores/validación, SQL Server, datos por servicio, transacciones y pruebas. El estudiante debe abrirlas y comprobarlas personalmente.\nVer bibliografía y Fuentes_Verificadas.md."),
        ("Qué se corrigió o descartó", "Metadata de validación de record DTO, cultura decimal de Range y extracción de IDs de Newman. Se actualizaron paquetes iniciales. No se implementó una transacción entre bases ni reserva automática de pedido; se documenta el alcance real."),
        ("Pruebas del asistente", "Tres soluciones compiladas sin errores ni advertencias. Colección Postman ejecutada con Newman: 90 peticiones y 98 aserciones sin fallas. Trece comprobaciones de integración, incluyendo colección, concurrencia y caída de servicios. Evidencia: docs/evidencias/resultado-integracion.json."),
        ("Incertidumbres y trazabilidad", "El prompt original no pidió fuentes oficiales; se conserva literalmente y se complementa con consulta directa. No se accedió al PEA ni a la Biblioteca ITSCO. El usuario confirmó el nombre Pablo Matias Roman Ruiz, 4.º B, y Matias Roman como nombre corto de entrega.")
    ]
    table = doc.tables[0]
    # Conserva la tabla de la plantilla, reemplazando sus textos de ejemplo.
    while len(table.rows) < len(filas) + 1:
        table.add_row()
    table.rows[0].cells[0].text = "Campo"
    table.rows[0].cells[1].text = "Registro de esta sesión"
    for row, (label, value) in zip(list(table.rows)[1:], filas):
        row.cells[0].text, row.cells[1].text = label, value
    table.autofit = False
    for row in table.rows:
        row.cells[0].width, row.cells[1].width = Cm(4), Cm(12)
        for cell in row.cells:
            for paragraph in cell.paragraphs:
                for run in paragraph.runs:
                    run.font.size = Pt(10)
    # Reemplaza las referencias preexistentes, que no corresponden todas a esta sesión.
    pars[7].text = "Bibliografía / referencias consultadas por el asistente"
    pars[8].text = referencias
    for paragraph in pars[9:]:
        paragraph.text = ""
    doc.add_heading("Consulta de fuentes y revisión personal", level=1)
    doc.add_paragraph("Para futuras consultas: solicita libros o fuentes oficiales verificables con autor, título, fecha y URL, y pide que se declare cualquier incertidumbre. Esta instrucción es una recomendación para consultas futuras, no un prompt adicional inventado de esta sesión.")
    doc.add_paragraph("Antes de entregar, el estudiante debe revisar, modificar cuando corresponda, probar y comprender el material. Las pruebas del asistente no sustituyen esa revisión personal.")
    for texto in ["Fecha de revisión personal: ____________________", "Nivel/paralelo: 4.º B", "Cambios propios y justificación: ________________________________________", "Fuentes abiertas personalmente: ________________________________________", "Resultado de mi ejecución en Postman: __________________________________", "Firma del estudiante: ____________________"]:
        doc.add_paragraph(texto)
    destino = DOCS / "Registro_IA_Matias_Roman.docx"
    doc.save(destino)

    # PDF con los mismos textos de la plantilla completada, en formato legible.
    elementos = []
    for child in doc.element.body:
        tag = child.tag.split("}")[-1]
        if tag == "p":
            # La API de python-docx conserva el texto y los saltos sin duplicarlos.
            from docx.text.paragraph import Paragraph as DocxParagraph
            parrafo = DocxParagraph(child, doc)
            texto = parrafo.text
            if not texto.strip():
                continue
            estilo = "TituloDoc" if texto == doc.paragraphs[0].text else "SeccionDoc" if parrafo.style.name.startswith("Heading") or texto.startswith("Bibliografía /") else "RegistroCuerpo"
            elementos.append(p(texto, estilo))
        elif tag == "tbl":
            for label, value in filas:
                elementos += [p(label, "RegistroCampo"), p(value, "RegistroCuerpo")]
    pdf(DOCS / "Registro_IA_Matias_Roman.pdf", elementos, "Registro de fuentes y uso responsable de IA — Pablo Matias Roman Ruiz — 4.º B")


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--plantilla", type=Path, default=Path.home() / "Downloads" / "Plantilla_Registro_Prompts_y_Fuentes.docx")
    args = parser.parse_args()
    registro(args.plantilla)
    renderSVG.drawToFile(arquitectura(), str(DOCS / "Arquitectura.svg"))
    markdown_pdf(DOCS / "Informe_Tecnico.md", DOCS / "Informe_Tecnico.pdf")
    print("Generados: declaración DOCX/PDF, informe PDF y arquitectura SVG.")
