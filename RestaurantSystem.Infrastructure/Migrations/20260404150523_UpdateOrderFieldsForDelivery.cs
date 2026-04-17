using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderFieldsForDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourierName",
                table: "Orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourierPhoneNumber",
                table: "Orders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Orders",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Orders",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ExternalOrderId",
                table: "Orders",
                column: "ExternalOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_ExternalOrderId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CourierName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CourierPhoneNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Orders");
        }
    }
}
