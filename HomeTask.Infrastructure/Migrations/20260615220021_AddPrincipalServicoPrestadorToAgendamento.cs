using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPrincipalServicoPrestadorToAgendamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PrincipalServicoPrestadorId",
                table: "Agendamentos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_PrincipalServicoPrestadorId",
                table: "Agendamentos",
                column: "PrincipalServicoPrestadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Servicos_PrincipalServicoPrestadorId",
                table: "Agendamentos",
                column: "PrincipalServicoPrestadorId",
                principalTable: "Servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Servicos_PrincipalServicoPrestadorId",
                table: "Agendamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_PrincipalServicoPrestadorId",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "PrincipalServicoPrestadorId",
                table: "Agendamentos");
        }
    }
}
