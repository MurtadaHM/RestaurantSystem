using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeAdvancedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuItemIngredients",
                table: "MenuItemIngredients");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "MenuItemIngredients",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "MenuItemIngredients",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MenuItemIngredients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOptional",
                table: "MenuItemIngredients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "MenuItemIngredients",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MenuItemIngredients",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WastePercentage",
                table: "MenuItemIngredients",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuItemIngredients",
                table: "MenuItemIngredients",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemIngredients_MenuItemId_IngredientId",
                table: "MenuItemIngredients",
                columns: new[] { "MenuItemId", "IngredientId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuItemIngredients",
                table: "MenuItemIngredients");

            migrationBuilder.DropIndex(
                name: "IX_MenuItemIngredients_MenuItemId_IngredientId",
                table: "MenuItemIngredients");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "MenuItemIngredients");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MenuItemIngredients");

            migrationBuilder.DropColumn(
                name: "IsOptional",
                table: "MenuItemIngredients");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "MenuItemIngredients");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MenuItemIngredients");

            migrationBuilder.DropColumn(
                name: "WastePercentage",
                table: "MenuItemIngredients");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "MenuItemIngredients",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuItemIngredients",
                table: "MenuItemIngredients",
                columns: new[] { "MenuItemId", "IngredientId" });
        }
    }
}
