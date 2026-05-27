using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FooKit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveRoleToDedicatedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create Roles table
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            // 2. Seed Roles data
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "System Administrator role", "Admin" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Standard user role", "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            // 3. Add RoleId column as nullable temporarily
            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            // 4. Migrate existing data from string Role to Guid RoleId
            migrationBuilder.Sql("UPDATE Users SET RoleId = '00000000-0000-0000-0000-000000000001' WHERE Role = 'Admin'");
            migrationBuilder.Sql("UPDATE Users SET RoleId = '00000000-0000-0000-0000-000000000002' WHERE Role <> 'Admin' OR Role IS NULL");

            // 5. Drop the old Role column
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            // 6. Make RoleId column non-nullable
            migrationBuilder.AlterColumn<Guid>(
                name: "RoleId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            // 7. Add foreign key
            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                table: "Users");

            // 1. Add back Role string column (nullable temporarily)
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            // 2. Populate Role string column from RoleId
            migrationBuilder.Sql("UPDATE Users SET Role = 'Admin' WHERE RoleId = '00000000-0000-0000-0000-000000000001'");
            migrationBuilder.Sql("UPDATE Users SET Role = 'User' WHERE RoleId = '00000000-0000-0000-0000-000000000002'");

            // 3. Make Role column non-nullable
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // 4. Drop RoleId column
            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Users");

            // 5. Drop Roles table
            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
