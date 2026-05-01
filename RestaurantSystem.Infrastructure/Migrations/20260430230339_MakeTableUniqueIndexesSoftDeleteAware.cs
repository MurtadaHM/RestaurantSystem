using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeTableUniqueIndexesSoftDeleteAware : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tables_TableNumber",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Tables_Code",
                table: "Tables");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_TableNumber",
                table: "Tables",
                column: "TableNumber",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_Code",
                table: "Tables",
                column: "Code",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tables_TableNumber",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Tables_Code",
                table: "Tables");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_TableNumber",
                table: "Tables",
                column: "TableNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tables_Code",
                table: "Tables",
                column: "Code",
                unique: true);
        }
    }
}