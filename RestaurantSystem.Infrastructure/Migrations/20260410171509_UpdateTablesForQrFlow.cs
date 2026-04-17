using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablesForQrFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Tables",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Tables",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FloorNumber",
                table: "Tables",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Tables",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOrderingEnabled",
                table: "Tables",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Zone",
                table: "Tables",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "Code", "FloorNumber", "IsActive", "IsOrderingEnabled", "Zone" },
                values: new object[] { "", null, true, true, null });

            migrationBuilder.UpdateData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "Code", "FloorNumber", "IsActive", "IsOrderingEnabled", "Zone" },
                values: new object[] { "", null, true, true, null });

            migrationBuilder.UpdateData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "Code", "FloorNumber", "IsActive", "IsOrderingEnabled", "Zone" },
                values: new object[] { "", null, true, true, null });

            migrationBuilder.UpdateData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "Code", "FloorNumber", "IsActive", "IsOrderingEnabled", "Zone" },
                values: new object[] { "", null, true, true, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "FloorNumber",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "IsOrderingEnabled",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "Zone",
                table: "Tables");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Tables",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
