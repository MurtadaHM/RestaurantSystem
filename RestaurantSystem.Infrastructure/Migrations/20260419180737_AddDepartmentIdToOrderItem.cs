using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentIdToOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "OrderItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_DepartmentId",
                table: "OrderItems",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Departments_DepartmentId",
                table: "OrderItems",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Departments_DepartmentId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_DepartmentId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "OrderItems");
        }
    }
}
