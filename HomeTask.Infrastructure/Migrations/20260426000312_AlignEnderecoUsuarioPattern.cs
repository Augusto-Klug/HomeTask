using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlignEnderecoUsuarioPattern : Migration
    {
        private static void ExecuteIgnoringMySqlError(MigrationBuilder migrationBuilder, string procedureName, int errorCode, string statement)
        {
            migrationBuilder.Sql(
                $$"""
                CREATE PROCEDURE `{{procedureName}}`()
                BEGIN
                    DECLARE CONTINUE HANDLER FOR {{errorCode}} BEGIN END;
                    {{statement}}
                END
                """,
                suppressTransaction: true);

            migrationBuilder.Sql($"CALL `{procedureName}`();", suppressTransaction: true);
            migrationBuilder.Sql($"DROP PROCEDURE `{procedureName}`;", suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ExecuteIgnoringMySqlError(
                migrationBuilder,
                "drop_fk_enderecos_usuarios",
                1091,
                "ALTER TABLE `Enderecos` DROP FOREIGN KEY `FK_Enderecos_Usuarios_UsuarioId`;");

            ExecuteIgnoringMySqlError(
                migrationBuilder,
                "drop_ix_enderecos_usuarioid",
                1091,
                "ALTER TABLE `Enderecos` DROP INDEX `IX_Enderecos_UsuarioId`;");

            ExecuteIgnoringMySqlError(
                migrationBuilder,
                "drop_fk_usuarios_enderecos",
                1091,
                "ALTER TABLE `Usuarios` DROP FOREIGN KEY `FK_Usuarios_Enderecos_EnderecoId`;");

            ExecuteIgnoringMySqlError(
                migrationBuilder,
                "drop_ix_usuarios_enderecoid",
                1091,
                "ALTER TABLE `Usuarios` DROP INDEX `IX_Usuarios_EnderecoId`;");

            ExecuteIgnoringMySqlError(
                migrationBuilder,
                "drop_col_usuarios_enderecoid",
                1091,
                "ALTER TABLE `Usuarios` DROP COLUMN `EnderecoId`;");

            migrationBuilder.CreateIndex(
                name: "IX_Enderecos_UsuarioId",
                table: "Enderecos",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Enderecos_Usuarios_UsuarioId",
                table: "Enderecos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ExecuteIgnoringMySqlError(
                migrationBuilder,
                "down_drop_fk_enderecos_usuarios",
                1091,
                "ALTER TABLE `Enderecos` DROP FOREIGN KEY `FK_Enderecos_Usuarios_UsuarioId`;");

            ExecuteIgnoringMySqlError(
                migrationBuilder,
                "down_drop_ix_enderecos_usuarioid",
                1091,
                "ALTER TABLE `Enderecos` DROP INDEX `IX_Enderecos_UsuarioId`;");

            migrationBuilder.AddColumn<Guid>(
                name: "EnderecoId",
                table: "Usuarios",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EnderecoId",
                table: "Usuarios",
                column: "EnderecoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Enderecos_EnderecoId",
                table: "Usuarios",
                column: "EnderecoId",
                principalTable: "Enderecos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.CreateIndex(
                name: "IX_Enderecos_UsuarioId",
                table: "Enderecos",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enderecos_Usuarios_UsuarioId",
                table: "Enderecos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
