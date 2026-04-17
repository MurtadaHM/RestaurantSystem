using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTeam6PartnerFieldsToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPartnerSyncDate",
                table: "Orders",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerOrderId",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerRestaurantId",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerSource",
                table: "Orders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPartnerSyncDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PartnerOrderId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PartnerRestaurantId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PartnerSource",
                table: "Orders");
        }
    }
}
