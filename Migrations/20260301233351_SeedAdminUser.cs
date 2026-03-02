using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoWasteOfMoney.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "PersonId", "Role", "UpdatedAt" },
                values: new object[] { new Guid("44444444-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$0nBntBvA262C.DttHkY8VOp1QEqI15hGq1Z4PofM4g/k3QxToz4/O", new Guid("11111111-0000-0000-0000-000000000001"), "Admin", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("44444444-0000-0000-0000-000000000001"));
        }
    }
}
