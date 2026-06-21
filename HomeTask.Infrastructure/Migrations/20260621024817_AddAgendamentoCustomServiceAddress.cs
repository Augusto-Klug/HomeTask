using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAgendamentoCustomServiceAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnderecoDescricao",
                table: "Agendamentos",
                type: "varchar(300)",
                maxLength: 300,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnderecoDescricao",
                table: "Agendamentos");
        }
    }
}
