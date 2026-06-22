using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPrestadorSuspensaoAutomaticaPorBaixaAvaliacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataFimSuspensao",
                table: "Prestadores",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInicioSuspensao",
                table: "Prestadores",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataPrimeiraNotificacaoBaixaAvaliacao",
                table: "Prestadores",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalAvaliacoesNaNotificacao",
                table: "Prestadores",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataFimSuspensao",
                table: "Prestadores");

            migrationBuilder.DropColumn(
                name: "DataInicioSuspensao",
                table: "Prestadores");

            migrationBuilder.DropColumn(
                name: "DataPrimeiraNotificacaoBaixaAvaliacao",
                table: "Prestadores");

            migrationBuilder.DropColumn(
                name: "TotalAvaliacoesNaNotificacao",
                table: "Prestadores");
        }
    }
}
