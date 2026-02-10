using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NoWasteOfMoney.Migrations
{
    /// <inheritdoc />
    public partial class SeedMvp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Movements",
                columns: new[] { "Id", "Description", "MovementTypeId", "Name" },
                values: new object[,]
                {
                    { 1, "Ficar grande", 1, "Academia" },
                    { 2, "Receba inteligencia", 1, "Pos graduacao" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "Id", "Email", "FirstName", "LastName" },
                values: new object[] { 1, "adimin@semPerdaDeDinheiro.com", "Pessoa", "Top" });

            migrationBuilder.InsertData(
                table: "MonthMovements",
                columns: new[] { "Id", "Month", "MovementId", "PersonId", "Value", "Year" },
                values: new object[] { 1, 2, 1, 1, 0m, 2026 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MonthMovements",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Movements",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Movements",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
