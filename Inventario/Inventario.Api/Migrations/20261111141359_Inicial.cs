using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventario.Api.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bodega",
                columns: table => new
                {
                    IdBodega = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bodega", x => x.IdBodega);
                });

            migrationBuilder.CreateTable(
                name: "Existencia",
                columns: table => new
                {
                    IdExistencia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdBodega = table.Column<int>(type: "int", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Existencia", x => x.IdExistencia);
                    table.CheckConstraint("CK_Existencia_Stock", "[Stock] >= 0");
                    table.ForeignKey(
                        name: "FK_Existencia_Bodega_IdBodega",
                        column: x => x.IdBodega,
                        principalTable: "Bodega",
                        principalColumn: "IdBodega",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientoInventario",
                columns: table => new
                {
                    IdMovimiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdBodega = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientoInventario", x => x.IdMovimiento);
                    table.CheckConstraint("CK_Movimiento_Cantidad", "[Cantidad] > 0");
                    table.CheckConstraint("CK_Movimiento_Tipo", "[Tipo] IN ('Entrada','Salida')");
                    table.ForeignKey(
                        name: "FK_MovimientoInventario_Bodega_IdBodega",
                        column: x => x.IdBodega,
                        principalTable: "Bodega",
                        principalColumn: "IdBodega",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservaStock",
                columns: table => new
                {
                    IdReserva = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdBodega = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservaStock", x => x.IdReserva);
                    table.CheckConstraint("CK_Reserva_Cantidad", "[Cantidad] > 0");
                    table.CheckConstraint("CK_Reserva_Estado", "[Estado] IN ('Activa','Cancelada','Consumida')");
                    table.ForeignKey(
                        name: "FK_ReservaStock_Bodega_IdBodega",
                        column: x => x.IdBodega,
                        principalTable: "Bodega",
                        principalColumn: "IdBodega",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bodega_Nombre",
                table: "Bodega",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Existencia_IdBodega",
                table: "Existencia",
                column: "IdBodega");

            migrationBuilder.CreateIndex(
                name: "IX_Existencia_IdProducto_IdBodega",
                table: "Existencia",
                columns: new[] { "IdProducto", "IdBodega" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoInventario_IdBodega",
                table: "MovimientoInventario",
                column: "IdBodega");

            migrationBuilder.CreateIndex(
                name: "IX_ReservaStock_IdBodega",
                table: "ReservaStock",
                column: "IdBodega");

            migrationBuilder.CreateIndex(
                name: "IX_ReservaStock_IdProducto_IdBodega_Estado",
                table: "ReservaStock",
                columns: new[] { "IdProducto", "IdBodega", "Estado" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Existencia");

            migrationBuilder.DropTable(
                name: "MovimientoInventario");

            migrationBuilder.DropTable(
                name: "ReservaStock");

            migrationBuilder.DropTable(
                name: "Bodega");
        }
    }
}
