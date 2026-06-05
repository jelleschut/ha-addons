using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HAShoppingList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NamingUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WeeklyRecurring",
                table: "Products",
                newName: "IsWeeklyRecurring");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsWeeklyRecurring",
                table: "Products",
                newName: "WeeklyRecurring");
        }
    }
}
