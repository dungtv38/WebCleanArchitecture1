
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "PasswordHash", "PhoneNumber", "Role", "UpdatedAt" },
                values: new object[] { new Guid("eec4b862-8bba-417c-904a-b926c33a7899"), new DateTime(2026, 3, 31, 17, 53, 8, 545, DateTimeKind.Local).AddTicks(6697), "admin@hotel.com", "System Admin", "hashed_password", "0869075546", 1, null });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "Address", "City", "CreatedAt", "Description", "Name", "OwnerId", "UpdatedAt" },
                values: new object[] { new Guid("d72221bf-e4a9-4610-b152-1882ea22fe90"), "123 Ly Thuong Kiet", "Hà Nội", new DateTime(2026, 3, 31, 17, 53, 8, 545, DateTimeKind.Local).AddTicks(6867), "Khách sạn trung tâm thành phố", "Grand Central Hotel", new Guid("eec4b862-8bba-417c-904a-b926c33a7899"), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: new Guid("d72221bf-e4a9-4610-b152-1882ea22fe90"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("eec4b862-8bba-417c-904a-b926c33a7899"));
        }
    }
}
