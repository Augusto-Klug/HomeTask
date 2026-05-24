using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StoreBillingFormatAsInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnidadeCobrancaNovo",
                table: "Servicos",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.Sql(
                """
                UPDATE `Servicos`
                SET `UnidadeCobrancaNovo` = CASE `UnidadeCobranca`
                    WHEN 'por_hora' THEN 1
                    WHEN 'a_combinar' THEN 3
                    ELSE 2
                END;
                """);

            migrationBuilder.DropColumn(
                name: "UnidadeCobranca",
                table: "Servicos");

            migrationBuilder.RenameColumn(
                name: "UnidadeCobrancaNovo",
                table: "Servicos",
                newName: "UnidadeCobranca");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnidadeCobrancaTexto",
                table: "Servicos",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "total")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.Sql(
                """
                UPDATE `Servicos`
                SET `UnidadeCobrancaTexto` = CASE `UnidadeCobranca`
                    WHEN 1 THEN 'por_hora'
                    WHEN 3 THEN 'a_combinar'
                    ELSE 'total'
                END;
                """);

            migrationBuilder.DropColumn(
                name: "UnidadeCobranca",
                table: "Servicos");

            migrationBuilder.RenameColumn(
                name: "UnidadeCobrancaTexto",
                table: "Servicos",
                newName: "UnidadeCobranca");
        }
    }
}
