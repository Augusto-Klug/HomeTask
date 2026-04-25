using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SplitServicoEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgendamentoServicos_ServicosOferecidos_ServicoOferecidoId",
                table: "AgendamentoServicos");

            migrationBuilder.DropTable(
                name: "ServicosOferecidos");

            migrationBuilder.RenameColumn(
                name: "ServicoOferecidoId",
                table: "AgendamentoServicos",
                newName: "ServicoBaseId");

            migrationBuilder.RenameIndex(
                name: "IX_AgendamentoServicos_ServicoOferecidoId",
                table: "AgendamentoServicos",
                newName: "IX_AgendamentoServicos_ServicoBaseId");

            migrationBuilder.CreateTable(
                name: "Servicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CategoriaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DataCriacao = table.Column<DateTime>(type: "datetime", nullable: false),
                    Ativo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Titulo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrecoBase = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UnidadeCobranca = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoAnuncio = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DataDesejada = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PrestadorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DuracaoEstimadaMinutos = table.Column<int>(type: "int", nullable: true),
                    AceitaPagamentoAposFinalizacao = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Servicos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Servicos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Servicos_Prestadores_PrestadorId",
                        column: x => x.PrestadorId,
                        principalTable: "Prestadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_CategoriaId",
                table: "Servicos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_ClienteId",
                table: "Servicos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_PrestadorId",
                table: "Servicos",
                column: "PrestadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AgendamentoServicos_Servicos_ServicoBaseId",
                table: "AgendamentoServicos",
                column: "ServicoBaseId",
                principalTable: "Servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgendamentoServicos_Servicos_ServicoBaseId",
                table: "AgendamentoServicos");

            migrationBuilder.DropTable(
                name: "Servicos");

            migrationBuilder.RenameColumn(
                name: "ServicoBaseId",
                table: "AgendamentoServicos",
                newName: "ServicoOferecidoId");

            migrationBuilder.RenameIndex(
                name: "IX_AgendamentoServicos_ServicoBaseId",
                table: "AgendamentoServicos",
                newName: "IX_AgendamentoServicos_ServicoOferecidoId");

            migrationBuilder.CreateTable(
                name: "ServicosOferecidos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CategoriaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ClienteId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    PrestadorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Ativo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DuracaoEstimadaMinutos = table.Column<int>(type: "int", nullable: true),
                    PrecoBase = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TipoAnuncio = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UnidadeCobranca = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicosOferecidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicosOferecidos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServicosOferecidos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ServicosOferecidos_Prestadores_PrestadorId",
                        column: x => x.PrestadorId,
                        principalTable: "Prestadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ServicosOferecidos_CategoriaId",
                table: "ServicosOferecidos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicosOferecidos_ClienteId",
                table: "ServicosOferecidos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicosOferecidos_PrestadorId",
                table: "ServicosOferecidos",
                column: "PrestadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AgendamentoServicos_ServicosOferecidos_ServicoOferecidoId",
                table: "AgendamentoServicos",
                column: "ServicoOferecidoId",
                principalTable: "ServicosOferecidos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
