using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GroceryListApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "Username" },
                values: new object[] { 1, "test@example.org", "Test user", "test" });

            migrationBuilder.InsertData(
                table: "Stores",
                columns: new[] { "Id", "Description", "Name", "UserId" },
                values: new object[] { 1, "An example grocery store.", "Example Groceries Inc.", 1 });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "IsComplete", "StoreId", "Title", "UserId" },
                values: new object[,]
                {
                    { 1, false, 1, "Potatoes", 1 },
                    { 2, false, 1, "Tomatoes", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
