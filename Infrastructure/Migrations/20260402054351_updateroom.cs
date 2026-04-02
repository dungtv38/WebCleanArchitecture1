using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateroom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rooms_RoomTypeId",
                table: "Rooms");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Rooms",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: new Guid("d72221bf-e4a9-4610-b152-1882ea22fe90"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 2, 12, 43, 50, 967, DateTimeKind.Local).AddTicks(6789));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("eec4b862-8bba-417c-904a-b926c33a7899"),
                column: "CreatedAt",
                value: new DateTime(2026, 4, 2, 12, 43, 50, 967, DateTimeKind.Local).AddTicks(6653));

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomTypeId_RoomNumber",
                table: "Rooms",
                columns: new[] { "RoomTypeId", "RoomNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rooms_RoomTypeId_RoomNumber",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Rooms");

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: new Guid("d72221bf-e4a9-4610-b152-1882ea22fe90"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 31, 17, 53, 8, 545, DateTimeKind.Local).AddTicks(6867));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("eec4b862-8bba-417c-904a-b926c33a7899"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 31, 17, 53, 8, 545, DateTimeKind.Local).AddTicks(6697));

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomTypeId",
                table: "Rooms",
                column: "RoomTypeId");
        }
    }
}
