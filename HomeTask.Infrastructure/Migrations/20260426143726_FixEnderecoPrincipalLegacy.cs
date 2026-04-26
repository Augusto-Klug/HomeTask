using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeTask.Infrastructure.Migrations
{
    /// <inheritdoc />
public partial class FixEnderecoPrincipalLegacy : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            SET @exists_principal := (
                SELECT COUNT(*)
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE()
                  AND TABLE_NAME = 'Enderecos'
                  AND COLUMN_NAME = 'Principal'
            );
            """);

        migrationBuilder.Sql(
            """
            SET @drop_principal_sql := IF(
                @exists_principal > 0,
                'ALTER TABLE `Enderecos` DROP COLUMN `Principal`',
                'SELECT 1'
            );
            """);

        migrationBuilder.Sql("PREPARE drop_principal_stmt FROM @drop_principal_sql;");
        migrationBuilder.Sql("EXECUTE drop_principal_stmt;");
        migrationBuilder.Sql("DEALLOCATE PREPARE drop_principal_stmt;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            SET @exists_principal := (
                SELECT COUNT(*)
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE()
                  AND TABLE_NAME = 'Enderecos'
                  AND COLUMN_NAME = 'Principal'
            );
            """);

        migrationBuilder.Sql(
            """
            SET @add_principal_sql := IF(
                @exists_principal = 0,
                'ALTER TABLE `Enderecos` ADD COLUMN `Principal` tinyint(1) NOT NULL DEFAULT 0',
                'SELECT 1'
            );
            """);

        migrationBuilder.Sql("PREPARE add_principal_stmt FROM @add_principal_sql;");
        migrationBuilder.Sql("EXECUTE add_principal_stmt;");
        migrationBuilder.Sql("DEALLOCATE PREPARE add_principal_stmt;");
    }
}
}
