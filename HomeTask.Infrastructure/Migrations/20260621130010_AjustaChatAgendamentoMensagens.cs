using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjustaChatAgendamentoMensagens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ConversaId",
                table: "Mensagens",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Mensagens_AgendamentoId_DataEnvio",
                table: "Mensagens",
                columns: new[] { "AgendamentoId", "DataEnvio" });

            migrationBuilder.CreateIndex(
                name: "IX_Mensagens_DataEnvio",
                table: "Mensagens",
                column: "DataEnvio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Mensagens_AgendamentoId_DataEnvio",
                table: "Mensagens");

            migrationBuilder.DropIndex(
                name: "IX_Mensagens_DataEnvio",
                table: "Mensagens");

            migrationBuilder.AlterColumn<Guid>(
                name: "ConversaId",
                table: "Mensagens",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

        }
    }
}
