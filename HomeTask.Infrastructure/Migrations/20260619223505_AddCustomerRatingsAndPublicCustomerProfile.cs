using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerRatingsAndPublicCustomerProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MediaAvaliacoes",
                table: "Clientes",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalAvaliacoes",
                table: "Clientes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AvaliacoesClientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AgendamentoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ClienteId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PrestadorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Nota = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataAvaliacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Visivel = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvaliacoesClientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvaliacoesClientes_Agendamentos_AgendamentoId",
                        column: x => x.AgendamentoId,
                        principalTable: "Agendamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AvaliacoesClientes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AvaliacoesClientes_Prestadores_PrestadorId",
                        column: x => x.PrestadorId,
                        principalTable: "Prestadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacoesClientes_AgendamentoId",
                table: "AvaliacoesClientes",
                column: "AgendamentoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacoesClientes_ClienteId",
                table: "AvaliacoesClientes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacoesClientes_PrestadorId",
                table: "AvaliacoesClientes",
                column: "PrestadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvaliacoesClientes");

            migrationBuilder.DropColumn(
                name: "MediaAvaliacoes",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "TotalAvaliacoes",
                table: "Clientes");
        }
    }
}
