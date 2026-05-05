using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderDepartmentProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS "OrderDepartmentProgresses" (
                    "Id" uuid NOT NULL,
                    "OrderId" uuid NOT NULL,
                    "DepartmentId" uuid NOT NULL,
                    "Status" text NOT NULL,
                    "StartedAt" timestamp with time zone,
                    "ReadyAt" timestamp with time zone,
                    "Notes" character varying(500),
                    "CreatedAt" timestamp with time zone NOT NULL,
                    "UpdatedAt" timestamp with time zone,
                    "IsDeleted" boolean NOT NULL,
                    "DeletedAt" timestamp with time zone,
                    CONSTRAINT "PK_OrderDepartmentProgresses" PRIMARY KEY ("Id"),
                    CONSTRAINT "FK_OrderDepartmentProgresses_Departments_DepartmentId"
                        FOREIGN KEY ("DepartmentId") REFERENCES "Departments" ("Id") ON DELETE RESTRICT,
                    CONSTRAINT "FK_OrderDepartmentProgresses_Orders_OrderId"
                        FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE
                );
                """);

            migrationBuilder.Sql("""
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_OrderDepartmentProgress_OrderId_DepartmentId"
                ON "OrderDepartmentProgresses" ("OrderId", "DepartmentId");
                """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS "IX_OrderDepartmentProgresses_DepartmentId"
                ON "OrderDepartmentProgresses" ("DepartmentId");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP TABLE IF EXISTS "OrderDepartmentProgresses";
                """);
        }
    }
}