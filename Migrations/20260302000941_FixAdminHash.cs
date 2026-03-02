using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoWasteOfMoney.Migrations
{
    /// <inheritdoc />
    public partial class FixAdminHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("44444444-0000-0000-0000-000000000001"),
                column: "PasswordHash",
                value: "$2a$11$VUJroA/DlBXzFO0E7G02vufDNSHx/s220G4FS1ysf3FZgG6TsGmH6");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("44444444-0000-0000-0000-000000000001"),
                column: "PasswordHash",
                value: "$2a$11$0nBntBvA262C.DttHkY8VOp1QEqI15hGq1Z4PofM4g/k3QxToz4/O");
        }
    }
}
