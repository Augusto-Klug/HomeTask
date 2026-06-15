using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDoubleRatingsAndPublicProviderProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nota",
                table: "Avaliacoes",
                newName: "NotaServico");

            migrationBuilder.AddColumn<decimal>(
                name: "MediaAvaliacoes",
                table: "Servicos",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalAvaliacoes",
                table: "Servicos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotaPrestador",
                table: "Avaliacoes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ServicoPrestadorId",
                table: "Avaliacoes",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_ServicoPrestadorId",
                table: "Avaliacoes",
                column: "ServicoPrestadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacoes_Servicos_ServicoPrestadorId",
                table: "Avaliacoes",
                column: "ServicoPrestadorId",
                principalTable: "Servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacoes_Servicos_ServicoPrestadorId",
                table: "Avaliacoes");

            migrationBuilder.DropIndex(
                name: "IX_Avaliacoes_ServicoPrestadorId",
                table: "Avaliacoes");

            migrationBuilder.DropColumn(
                name: "MediaAvaliacoes",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "TotalAvaliacoes",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "NotaPrestador",
                table: "Avaliacoes");

            migrationBuilder.DropColumn(
                name: "ServicoPrestadorId",
                table: "Avaliacoes");

            migrationBuilder.RenameColumn(
                name: "NotaServico",
                table: "Avaliacoes",
                newName: "Nota");
        }
    }
}
