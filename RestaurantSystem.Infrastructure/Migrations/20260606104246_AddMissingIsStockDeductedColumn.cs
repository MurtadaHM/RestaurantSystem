using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingIsStockDeductedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE "Orders"
                ADD COLUMN IF NOT EXISTS "IsStockDeducted"
                boolean NOT NULL DEFAULT FALSE;
                """
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE "Orders"
                DROP COLUMN IF EXISTS "IsStockDeducted";
                """
            );
        }
    }
}