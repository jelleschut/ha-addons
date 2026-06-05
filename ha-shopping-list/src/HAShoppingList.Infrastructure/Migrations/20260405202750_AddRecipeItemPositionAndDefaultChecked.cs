using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HAShoppingList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeItemPositionAndDefaultChecked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultChecked",
                table: "RecipeItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "RecipeItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefaultChecked",
                table: "RecipeItems");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "RecipeItems");
        }
    }
}
