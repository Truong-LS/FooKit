using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FooKit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeAndAffiliateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "DishCaches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalApiId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DietaryTagsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiredToolsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RawIngredientsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastFetchedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishCaches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StandardIngredients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardIngredients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PriceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceCurrency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DurationInDays = table.Column<int>(type: "int", nullable: false),
                    FeaturesJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SuggestionRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetBudgetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TargetBudgetCurrency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DietaryRequirement = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AvailableToolsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuggestionRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuggestionRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDietaryPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DietaryType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDietaryPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDietaryPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTools",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToolName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTools_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AffiliateProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StandardIngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ProductUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CurrentPriceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentPriceCurrency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastUpdatedPriceAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliateProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AffiliateProducts_StandardIngredients_StandardIngredientId",
                        column: x => x.StandardIngredientId,
                        principalTable: "StandardIngredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IngredientDictionaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RawKeywordFromApi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StandardIngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientDictionaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngredientDictionaries_StandardIngredients_StandardIngredientId",
                        column: x => x.StandardIngredientId,
                        principalTable: "StandardIngredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_SubscriptionPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SuggestionResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuggestionRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DishCacheId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalEstimatedPriceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalEstimatedPriceCurrency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuggestionResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuggestionResults_DishCaches_DishCacheId",
                        column: x => x.DishCacheId,
                        principalTable: "DishCaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SuggestionResults_SuggestionRequests_SuggestionRequestId",
                        column: x => x.SuggestionRequestId,
                        principalTable: "SuggestionRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AffiliateProducts_StandardIngredientId",
                table: "AffiliateProducts",
                column: "StandardIngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientDictionaries_StandardIngredientId",
                table: "IngredientDictionaries",
                column: "StandardIngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_SuggestionRequests_UserId",
                table: "SuggestionRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SuggestionResults_DishCacheId",
                table: "SuggestionResults",
                column: "DishCacheId");

            migrationBuilder.CreateIndex(
                name: "IX_SuggestionResults_SuggestionRequestId",
                table: "SuggestionResults",
                column: "SuggestionRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDietaryPreferences_UserId",
                table: "UserDietaryPreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_PlanId",
                table: "UserSubscriptions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_UserId",
                table: "UserSubscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTools_UserId",
                table: "UserTools",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AffiliateProducts");

            migrationBuilder.DropTable(
                name: "IngredientDictionaries");

            migrationBuilder.DropTable(
                name: "SuggestionResults");

            migrationBuilder.DropTable(
                name: "UserDietaryPreferences");

            migrationBuilder.DropTable(
                name: "UserSubscriptions");

            migrationBuilder.DropTable(
                name: "UserTools");

            migrationBuilder.DropTable(
                name: "StandardIngredients");

            migrationBuilder.DropTable(
                name: "DishCaches");

            migrationBuilder.DropTable(
                name: "SuggestionRequests");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");
        }
    }
}
