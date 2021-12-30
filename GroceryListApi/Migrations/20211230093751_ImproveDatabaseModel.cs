using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroceryListApi.Migrations
{
    public partial class ImproveDatabaseModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Stores",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Username",
                table: "Users",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_UserId",
                table: "Stores",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_StoreId",
                table: "Items",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_UserId",
                table: "Items",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Stores_StoreId",
                table: "Items",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Users_UserId",
                table: "Items",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Users_UserId",
                table: "Stores",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Stores_StoreId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Users_UserId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Users_UserId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Stores_UserId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Items_StoreId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_UserId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Items");
        }
    }
}
