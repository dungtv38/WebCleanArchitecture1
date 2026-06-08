using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MovePriceAndGuestsToRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxGuests",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "PricePerNight",
                table: "RoomTypes");

            migrationBuilder.AddColumn<int>(
                name: "MaxGuests",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerNight",
                table: "Rooms",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: new Guid("d72221bf-e4a9-4610-b152-1882ea22fe90"),
                column: "CreatedAt",
                value: new DateTime(2026, 6, 8, 16, 52, 32, 262, DateTimeKind.Local).AddTicks(3161));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("eec4b862-8bba-417c-904a-b926c33a7899"),
                column: "CreatedAt",
                value: new DateTime(2026, 6, 8, 16, 52, 32, 262, DateTimeKind.Local).AddTicks(2970));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxGuests",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "PricePerNight",
                table: "Rooms");

            migrationBuilder.AddColumn<int>(
                name: "MaxGuests",
                table: "RoomTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerNight",
                table: "RoomTypes",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: new Guid("d72221bf-e4a9-4610-b152-1882ea22fe90"),
                column: "CreatedAt",
                value: new DateTime(2026, 6, 5, 15, 13, 35, 305, DateTimeKind.Local).AddTicks(2157));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("eec4b862-8bba-417c-904a-b926c33a7899"),
                column: "CreatedAt",
                value: new DateTime(2026, 6, 5, 15, 13, 35, 305, DateTimeKind.Local).AddTicks(1928));
        }
    }
}
