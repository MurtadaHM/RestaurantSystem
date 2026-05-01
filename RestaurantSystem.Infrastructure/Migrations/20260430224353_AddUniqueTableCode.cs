using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueTableCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Zone",
                table: "Tables",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Tables",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReservationDate",
                table: "Reservations",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.UpdateData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "Code", "FloorNumber", "Zone" },
                values: new object[] { "TBL-T1", 1, "الصالة الرئيسية" });

            migrationBuilder.UpdateData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "Code", "FloorNumber", "Zone" },
                values: new object[] { "TBL-T2", 1, "الصالة الرئيسية" });

            migrationBuilder.UpdateData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "Code", "FloorNumber", "Zone" },
                values: new object[] { "TBL-T3", 1, "الصالة الرئيسية" });

            migrationBuilder.UpdateData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "Code", "FloorNumber", "Zone" },
                values: new object[] { "TBL-T4", 1, "VIP" });

            migrationBuilder.CreateIndex(
                name: "IX_Tables_Code",
                table: "Tables",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tables_Code",
                table: "Tables");

            migrationBuilder.AlterColumn<string>(
                name: "Zone",
                table: "Tables",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Tables",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReservationDate",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.UpdateData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "Code", "FloorNumber", "Zone" },
                values: new object[] { "", null, null });

            migrationBuilder.UpdateData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "Code", "FloorNumber", "Zone" },
                values: new object[] { "", null, null });

            migrationBuilder.UpdateData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "Code", "FloorNumber", "Zone" },
                values: new object[] { "", null, null });

            migrationBuilder.UpdateData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                columns: new[] { "Code", "FloorNumber", "Zone" },
                values: new object[] { "", null, null });
        }
    }
}
