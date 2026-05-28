using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FooKit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIngredientFilters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000099"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "RoleId", "Username" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000099"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Default User", true, "$2a$11$bjBTzVjWIFb.5L7NGW5IruqxBiquPLihDJV05EQ1zKJezfF5XZzdC", new Guid("00000000-0000-0000-0000-000000000002"), "user" });
        }
    }
}
