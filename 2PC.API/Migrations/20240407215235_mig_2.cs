using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Coordinator.Migrations
{
    /// <inheritdoc />
    public partial class mig_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Nodes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("5b14d9fa-b213-436b-90c1-aae3f9576f7d"), "StockAPI" },
                    { new Guid("cf5ff207-a034-46d2-a036-4ed8009b36da"), "PaymentAPI" },
                    { new Guid("d76f5fd2-0b21-40df-b7e1-ce98089fd3f3"), "OrderAPI" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("5b14d9fa-b213-436b-90c1-aae3f9576f7d"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("cf5ff207-a034-46d2-a036-4ed8009b36da"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("d76f5fd2-0b21-40df-b7e1-ce98089fd3f3"));
        }
    }
}
