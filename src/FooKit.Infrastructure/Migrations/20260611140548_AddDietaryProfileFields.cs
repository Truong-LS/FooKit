using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FooKit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDietaryProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "WeeklyBudget",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserFavoriteCuisines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CuisineName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteCuisines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFavoriteCuisines_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteCuisines_UserId",
                table: "UserFavoriteCuisines",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFavoriteCuisines");

            migrationBuilder.DropColumn(
                name: "WeeklyBudget",
                table: "Users");
        }
    }
}
