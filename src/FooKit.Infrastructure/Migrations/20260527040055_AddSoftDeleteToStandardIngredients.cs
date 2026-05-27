using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FooKit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToStandardIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StandardIngredients",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StandardIngredients");
        }
    }
}
