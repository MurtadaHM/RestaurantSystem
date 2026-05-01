using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantSystem.Infrastructure.Migrations
{
    public partial class AlterReservationDateToTimestampWithoutTimeZone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Convert existing timestamptz values to local wall-clock timestamps.
            // Note: review the SQL; it assumes stored values are timestamptz and converts them to timestamp without time zone.
            migrationBuilder.Sql(
                "ALTER TABLE \"Reservations\" ALTER COLUMN \"ReservationDate\" TYPE timestamp without time zone USING (\"ReservationDate\" AT TIME ZONE 'UTC');");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReservationDate",
                table: "Reservations",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert — convert stored local timestamps back to timestamptz assuming they represent UTC wall-clock times.
            // Please review before applying to production.
            migrationBuilder.Sql(
                "ALTER TABLE \"Reservations\" ALTER COLUMN \"ReservationDate\" TYPE timestamp with time zone USING ((\"ReservationDate\") AT TIME ZONE 'UTC');");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReservationDate",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");
        }
    }
}