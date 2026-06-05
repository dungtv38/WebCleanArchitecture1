using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: new Guid("d72221bf-e4a9-4610-b152-1882ea22fe90"),
                column: "CreatedAt",
                value: new DateTime(2026, 5, 23, 18, 20, 21, 743, DateTimeKind.Local).AddTicks(8950));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("eec4b862-8bba-417c-904a-b926c33a7899"),
                column: "CreatedAt",
                value: new DateTime(2026, 5, 23, 18, 20, 21, 743, DateTimeKind.Local).AddTicks(8737));
        }
    }
}
