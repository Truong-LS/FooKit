USE [master]
GO
/****** Object:  Database [FooKit]    Script Date: 27/05/2026 10:30:00 SA ******/
CREATE DATABASE [FooKit]
 
GO
ALTER DATABASE [FooKit] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [FooKit].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [FooKit] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [FooKit] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [FooKit] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [FooKit] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [FooKit] SET ARITHABORT OFF 
GO
ALTER DATABASE [FooKit] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [FooKit] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [FooKit] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [FooKit] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [FooKit] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [FooKit] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [FooKit] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [FooKit] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [FooKit] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [FooKit] SET  ENABLE_BROKER 
GO
ALTER DATABASE [FooKit] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [FooKit] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [FooKit] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [FooKit] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [FooKit] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [FooKit] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [FooKit] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [FooKit] SET RECOVERY FULL 
GO
ALTER DATABASE [FooKit] SET  MULTI_USER 
GO
ALTER DATABASE [FooKit] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [FooKit] SET DB_CHAINING OFF 
GO
ALTER DATABASE [FooKit] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [FooKit] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [FooKit] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [FooKit] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'FooKit', N'ON'
GO
ALTER DATABASE [FooKit] SET QUERY_STORE = ON
GO
ALTER DATABASE [FooKit] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [FooKit]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AffiliateProducts]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AffiliateProducts](
	[Id] [uniqueidentifier] NOT NULL,
	[StandardIngredientId] [uniqueidentifier] NOT NULL,
	[ProductName] [nvarchar](255) NOT NULL,
	[ProductUrl] [nvarchar](1000) NOT NULL,
	[CurrentPriceAmount] [decimal](18, 2) NOT NULL,
	[CurrentPriceCurrency] [nvarchar](10) NOT NULL,
	[Platform] [nvarchar](100) NOT NULL,
	[LastUpdatedPriceAt] [datetime2](7) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_AffiliateProducts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DishCaches]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DishCaches](
	[Id] [uniqueidentifier] NOT NULL,
	[ExternalApiId] [nvarchar](100) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[ImageUrl] [nvarchar](1000) NOT NULL,
	[DietaryTagsJson] [nvarchar](max) NOT NULL,
	[RequiredToolsJson] [nvarchar](max) NOT NULL,
	[RawIngredientsJson] [nvarchar](max) NOT NULL,
	[LastFetchedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_DishCaches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IngredientDictionaries]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IngredientDictionaries](
	[Id] [uniqueidentifier] NOT NULL,
	[RawKeywordFromApi] [nvarchar](255) NOT NULL,
	[StandardIngredientId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_IngredientDictionaries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[SubscriptionPlanId] [uniqueidentifier] NOT NULL,
	[TransactionRef] [nvarchar](50) NOT NULL,
	[Amount] [decimal](18, 0) NOT NULL,
	[OrderInfo] [nvarchar](255) NOT NULL,
	[Status] [int] NOT NULL,
	[VnPayTransactionNo] [nvarchar](50) NULL,
	[VnPayResponseCode] [nvarchar](10) NULL,
	[BankCode] [nvarchar](20) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[PaidAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RefreshTokens]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RefreshTokens](
	[Id] [uniqueidentifier] NOT NULL,
	[Token] [nvarchar](200) NOT NULL,
	[AddedDate] [datetime2](7) NOT NULL,
	[ExpiryDate] [datetime2](7) NOT NULL,
	[IsRevoked] [bit] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_RefreshTokens] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StandardIngredients]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StandardIngredients](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Category] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_StandardIngredients] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubscriptionPlans]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubscriptionPlans](
	[Id] [uniqueidentifier] NOT NULL,
	[PlanName] [nvarchar](100) NOT NULL,
	[PriceAmount] [decimal](18, 2) NOT NULL,
	[PriceCurrency] [nvarchar](10) NOT NULL,
	[DurationInDays] [int] NOT NULL,
	[FeaturesJson] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_SubscriptionPlans] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SuggestionRequests]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SuggestionRequests](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[TargetBudgetAmount] [decimal](18, 2) NOT NULL,
	[TargetBudgetCurrency] [nvarchar](10) NOT NULL,
	[DietaryRequirement] [nvarchar](50) NOT NULL,
	[AvailableToolsJson] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_SuggestionRequests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SuggestionResults]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SuggestionResults](
	[Id] [uniqueidentifier] NOT NULL,
	[SuggestionRequestId] [uniqueidentifier] NOT NULL,
	[DishCacheId] [uniqueidentifier] NOT NULL,
	[TotalEstimatedPriceAmount] [decimal](18, 2) NOT NULL,
	[TotalEstimatedPriceCurrency] [nvarchar](10) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_SuggestionResults] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ThirdPartyApiLogs]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ThirdPartyApiLogs](
	[Id] [uniqueidentifier] NOT NULL,
	[ServiceName] [nvarchar](max) NOT NULL,
	[Endpoint] [nvarchar](max) NOT NULL,
	[TokensUsed] [int] NOT NULL,
	[WasCacheHit] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ThirdPartyApiLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserAllergies]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAllergies](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[AllergenName] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_UserAllergies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserDietaryPreferences]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserDietaryPreferences](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[DietaryType] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_UserDietaryPreferences] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserHistories]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserHistories](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[DishName] [nvarchar](max) NOT NULL,
	[CookedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_UserHistories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserHomepageCaches]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserHomepageCaches](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[SerializedMenuData] [nvarchar](max) NOT NULL,
	[ExpirationTime] [datetime2](7) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_UserHomepageCaches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserLogins]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](256) NOT NULL,
	[ProviderDisplayName] [nvarchar](128) NULL,
	[UserId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_UserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[PasswordHash] [nvarchar](255) NULL,
	[Email] [nvarchar](256) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[FullName] [nvarchar](100) NULL,
	[IsActive] [bit] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserSubscriptions]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSubscriptions](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[PlanId] [uniqueidentifier] NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_UserSubscriptions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserTools]    Script Date: 27/05/2026 10:30:01 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserTools](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[ToolName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_UserTools] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260424033205_InitialCreate', N'9.0.15')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260514011848_AddRecipeAndAffiliateSchema', N'9.0.16')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260514135025_AddPaymentTable', N'9.0.16')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260523032619_AddHomepageCaching', N'9.0.16')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260527030457_AddAdminDashboardEntities', N'9.0.16')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260527031731_MoveRoleToDedicatedTable', N'9.0.16')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260527032148_updatedatabase', N'9.0.16')
GO
INSERT [dbo].[DishCaches] ([Id], [ExternalApiId], [Name], [ImageUrl], [DietaryTagsJson], [RequiredToolsJson], [RawIngredientsJson], [LastFetchedAt]) VALUES (N'9df22a44-952d-4623-a37e-2693cfac0464', N'118542121', N'Asparagus and Pea Soup: Real Convenience Food', N'https://img.spoonacular.com/recipes/716406-312x231.jpg', N'["gluten free","dairy free","paleolithic","lacto ovo vegetarian","primal","vegan"]', N'[""]', N'["1 bag of frozen organic asparagus (preferably thawed)","1T EVOO (extra virgin olive oil)","a couple of garlic cloves","1/2 onion","2-3c of frozen organic peas","1 box low-sodium vegetable broth"]', CAST(N'2026-05-23T04:22:59.0134308' AS DateTime2))
INSERT [dbo].[DishCaches] ([Id], [ExternalApiId], [Name], [ImageUrl], [DietaryTagsJson], [RequiredToolsJson], [RawIngredientsJson], [LastFetchedAt]) VALUES (N'e757104a-daf3-425a-93eb-422b4d6151be', N'-1716784315', N'Red Lentil Soup with Chicken and Turnips', N'https://img.spoonacular.com/recipes/715415-312x231.jpg', N'["gluten free","dairy free"]', N'[""]', N'["additional toppings: diced avocado, micro greens, chopped basil)","3 medium carrots, peeled and diced","3 celery stalks, diced","2 cups fully-cooked chicken breast, shredded (may be omitted for a vegetarian version)","\u00BD cup flat leaf Italian parsley, chopped (plus extra for garnish)","6 cloves of garlic, finely minced","2 tablespoons olive oil","28 ounce-can plum tomatoes, drained and rinsed, chopped","2 cups dried red lentils, rinsed","salt and black pepper, to taste","1 large turnip, peeled and diced","8 cups vegetable stock","1 medium yellow onion, diced"]', CAST(N'2026-05-23T04:22:58.6560845' AS DateTime2))
INSERT [dbo].[DishCaches] ([Id], [ExternalApiId], [Name], [ImageUrl], [DietaryTagsJson], [RequiredToolsJson], [RawIngredientsJson], [LastFetchedAt]) VALUES (N'1beb8076-8a4e-4b65-bf8d-6d6d02f3b0ec', N'-780260140', N'Garlicky Kale', N'https://img.spoonacular.com/recipes/644387-312x231.jpg', N'["gluten free","dairy free","paleolithic","lacto ovo vegetarian","primal","whole 30","vegan"]', N'[""]', N'["3 tablespoons balsamic vinegar","1 clove garlic, minced","1 bunch curly kale, stems removed and chopped","Olive oil"]', CAST(N'2026-05-23T04:22:59.0243581' AS DateTime2))
GO
INSERT [dbo].[RefreshTokens] ([Id], [Token], [AddedDate], [ExpiryDate], [IsRevoked], [UserId]) VALUES (N'ab40e9ca-8780-45c1-bdd2-201eb13d51b6', N'rGAkyaoyOs22I1XtjpIthYX1xoMF/eW9tBd/VWIQQBFapy36rEZRziOochPFfHPHaprzkTnzIby6gckI8kf1cw==', CAST(N'2026-05-23T03:49:38.7376500' AS DateTime2), CAST(N'2026-05-30T03:49:38.7376506' AS DateTime2), 1, N'5191a820-c43d-4a37-bf69-2ea5cfed4e69')
INSERT [dbo].[RefreshTokens] ([Id], [Token], [AddedDate], [ExpiryDate], [IsRevoked], [UserId]) VALUES (N'c1d148ed-f34a-4318-9601-286c89c80f40', N'ymEwhDQJMPY7pIVCE11yYQYfn+7S+ky3eUW7RkesqQ88IsZfL2Y1ML3/gv6bh3KToFT1xYldgPUcJi154EMiSQ==', CAST(N'2026-05-23T04:18:42.5033034' AS DateTime2), CAST(N'2026-05-30T04:18:42.5034249' AS DateTime2), 0, N'5191a820-c43d-4a37-bf69-2ea5cfed4e69')
INSERT [dbo].[RefreshTokens] ([Id], [Token], [AddedDate], [ExpiryDate], [IsRevoked], [UserId]) VALUES (N'cbdb5505-b8de-4bc3-9b8e-2a85e260c52b', N'TAE3+0ZW8ZaeA41vAqBfOD8Sx6fof+XD0aPwNzb0yTE5c4bGaIBC5pK0xv5ljWO1Rs+VgBIWRaiBIavJae9yrw==', CAST(N'2026-05-23T04:18:48.5823958' AS DateTime2), CAST(N'2026-05-30T04:18:48.5823962' AS DateTime2), 0, N'5191a820-c43d-4a37-bf69-2ea5cfed4e69')
INSERT [dbo].[RefreshTokens] ([Id], [Token], [AddedDate], [ExpiryDate], [IsRevoked], [UserId]) VALUES (N'c8643c7b-25e3-44cf-9a5b-645850f9f538', N'/OrGZFF2+TjeBRU8Obg7750jgzWjgovLpzZGrkoXCKXG3re8xVhQpea1B7t1DBeV7jWJ4lNP+VXZuyhKSuOHKQ==', CAST(N'2026-04-24T04:02:08.0051731' AS DateTime2), CAST(N'2026-05-01T04:02:08.0051734' AS DateTime2), 1, N'5191a820-c43d-4a37-bf69-2ea5cfed4e69')
INSERT [dbo].[RefreshTokens] ([Id], [Token], [AddedDate], [ExpiryDate], [IsRevoked], [UserId]) VALUES (N'ebb774f5-411a-4396-bae8-6513959506b0', N'wHMTcHz54u76cmRgvMVGM70gucIRVgE7pYIS5LevfOPfo0boIpGskaAePx0PKi8fjwrUxkr1dlZK8P8rCOe/vA==', CAST(N'2026-05-23T03:50:05.6859792' AS DateTime2), CAST(N'2026-05-30T03:50:05.6859797' AS DateTime2), 1, N'5191a820-c43d-4a37-bf69-2ea5cfed4e69')
INSERT [dbo].[RefreshTokens] ([Id], [Token], [AddedDate], [ExpiryDate], [IsRevoked], [UserId]) VALUES (N'b525240c-a57c-4c60-9984-69ea33e2b7b9', N'dnXqquL7pvvStRzuN+Tdl80RbTNjJ8z9cb51o5NpeiRp6ByHQdAFC71iLk/sBrhwrOkSsQGa45rXX246WOMp4g==', CAST(N'2026-04-24T03:58:16.7569871' AS DateTime2), CAST(N'2026-05-01T03:58:16.7570737' AS DateTime2), 1, N'5191a820-c43d-4a37-bf69-2ea5cfed4e69')
INSERT [dbo].[RefreshTokens] ([Id], [Token], [AddedDate], [ExpiryDate], [IsRevoked], [UserId]) VALUES (N'784df542-1464-492c-8a6d-8cfd6e0b7cd1', N'72SLHlDhhPs6eH67ZFXdChqlrea96hdOxuK+nmTg7s01AUO5GyN52hPa1qNdXp6Ube02mv3cQKpfRvXeqm+Huw==', CAST(N'2026-04-24T03:58:32.6849829' AS DateTime2), CAST(N'2026-05-01T03:58:32.6849838' AS DateTime2), 1, N'5191a820-c43d-4a37-bf69-2ea5cfed4e69')
INSERT [dbo].[RefreshTokens] ([Id], [Token], [AddedDate], [ExpiryDate], [IsRevoked], [UserId]) VALUES (N'c0bc3c59-e508-4af0-b765-906f4ab66e1f', N'/vcBJNKE57kB5PQ1Lz1HCsIV96VzlbxmXuW4QNw6U36ZkliCQ/Y672U3otCnHYiv9XPBbNou8F82YCepihw++A==', CAST(N'2026-05-23T03:43:34.4311793' AS DateTime2), CAST(N'2026-05-30T03:43:34.4313059' AS DateTime2), 1, N'5191a820-c43d-4a37-bf69-2ea5cfed4e69')
INSERT [dbo].[RefreshTokens] ([Id], [Token], [AddedDate], [ExpiryDate], [IsRevoked], [UserId]) VALUES (N'59104a88-56b7-48fa-a8ba-98d2b7af7ec4', N'N6vYnSFFuaunn/VWMy63GO6JDju+c25Yienx89RCTx4wcDVVkj1d4RafgGlv+08FQeVjx0bzR76mpvzclJkFEQ==', CAST(N'2026-05-23T04:35:27.0554215' AS DateTime2), CAST(N'2026-05-30T04:35:27.0555052' AS DateTime2), 0, N'5191a820-c43d-4a37-bf69-2ea5cfed4e69')
INSERT [dbo].[RefreshTokens] ([Id], [Token], [AddedDate], [ExpiryDate], [IsRevoked], [UserId]) VALUES (N'759e167f-86c6-4dd8-8085-9a1615a9aec4', N'/7CTGABm4lf1qQUBD00Ig7zWF7UThpIXwOYN9natyM0UMi302slkEOTcIi/58MDZgoXltOLka4OfnsBUTw/hGw==', CAST(N'2026-05-23T03:58:17.3479735' AS DateTime2), CAST(N'2026-05-30T03:58:17.3479739' AS DateTime2), 1, N'5191a820-c43d-4a37-bf69-2ea5cfed4e69')
INSERT [dbo].[RefreshTokens] ([Id], [Token], [AddedDate], [ExpiryDate], [IsRevoked], [UserId]) VALUES (N'80de42da-6143-40b4-b36a-b951b117eaa2', N'pkppXcNog74gdOTjepP53zlPlLAbkM87D3QVN1IUeguHwaeLndcxRhiDRVmd1adT7a05lTgOEBJMJRhQifg86w==', CAST(N'2026-04-24T03:58:37.8378327' AS DateTime2), CAST(N'2026-05-01T03:58:37.8378330' AS DateTime2), 1, N'5191a820-c43d-4a37-bf69-2ea5cfed4e69')
INSERT [dbo].[RefreshTokens] ([Id], [Token], [AddedDate], [ExpiryDate], [IsRevoked], [UserId]) VALUES (N'ca70326f-46a1-49f2-a4f4-bce16c36e1da', N'QGysq7FBpbj787ZcUQhiWZLq+rgrAHmT1xlxRob7WvqZ9u2Rm1dV+bfJaYaCQzyHYDPP+uKpHtWlngSWrDEl4w==', CAST(N'2026-05-23T03:56:20.9083433' AS DateTime2), CAST(N'2026-05-30T03:56:20.9083443' AS DateTime2), 1, N'5191a820-c43d-4a37-bf69-2ea5cfed4e69')
INSERT [dbo].[RefreshTokens] ([Id], [Token], [AddedDate], [ExpiryDate], [IsRevoked], [UserId]) VALUES (N'd0dd42f8-40ab-4e7b-a27b-e2603d5d0bcb', N'6mOsB2j9K+Wda8HqEpNgc/jsa62485M1LhUN8XiygJKZzR7AhQt+hx7C3bvXqdnxfjqAbpL207NeJfHt2a4Mgg==', CAST(N'2026-05-23T03:55:52.4004027' AS DateTime2), CAST(N'2026-05-30T03:55:52.4004031' AS DateTime2), 1, N'5191a820-c43d-4a37-bf69-2ea5cfed4e69')
INSERT [dbo].[RefreshTokens] ([Id], [Token], [AddedDate], [ExpiryDate], [IsRevoked], [UserId]) VALUES (N'f242975a-1aa0-4e4a-b95d-ed8c385d6334', N'mZ/wb31tAo7JWAfGNcqRH/XjI0W3gdFQq4zklotswzMG2lNAoOhwuRoabR4RAi0i0vv7iR/nrplMEbr5yCi1OQ==', CAST(N'2026-04-24T04:34:02.0295714' AS DateTime2), CAST(N'2026-05-01T04:34:02.0296607' AS DateTime2), 1, N'5191a820-c43d-4a37-bf69-2ea5cfed4e69')
GO
INSERT [dbo].[Roles] ([Id], [Name], [Description]) VALUES (N'00000000-0000-0000-0000-000000000001', N'Admin', N'System Administrator role')
INSERT [dbo].[Roles] ([Id], [Name], [Description]) VALUES (N'00000000-0000-0000-0000-000000000002', N'User', N'Standard user role')
GO
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'd6b1eded-ee6c-4b4d-897c-03106655fff8', N'Sữa đặc', N'Bơ sữa & Khác')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'ecbdcb50-8cca-427b-a9cd-043714235dba', N'Sữa chua', N'Bơ sữa & Khác')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'dd7fbea2-53e3-4d48-9c76-057739ff923a', N'Đùi gà', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'60329732-9fd7-43ef-9362-0716224c9f21', N'Bánh tráng', N'Tinh bột')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'de62b7b2-e4bf-4392-9f2b-0ab5356cf23d', N'Sa tế', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'970326a1-dda4-4f95-8c6f-0fb77312fc94', N'Thịt ba chỉ heo', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'181718fa-03a0-4957-ab59-0ffca7fcb331', N'Ớt tươi', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'd7d7e799-6fd6-4e68-8828-1366a9fddf83', N'Mật ong', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'a6747cf8-41bd-4740-ade5-142ca2ec453e', N'Nấm rơm', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'8213a21d-1858-4fdf-8e80-1511af7d953c', N'Giấm táo', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'3d86c895-d64b-472d-924f-1c015b9e693b', N'Trứng vịt', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'613ebd5f-29fd-4a59-bac2-1d77b954dec5', N'Phô mai con bò cười', N'Bơ sữa & Khác')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'a7a590fe-6c58-4559-9fc5-1f5dadbbe930', N'Sả', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'48bc8940-beb5-496a-b2e2-20402f5f6e35', N'Mì Ý (Pasta)', N'Tinh bột')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'7e24da31-9a02-44a3-8d9f-2264ccc9409f', N'Bột cà ri', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'055c1621-bb32-4d98-921a-250aa0eea7be', N'Đậu phụ (Đậu hũ)', N'Bơ sữa & Khác')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'928444b8-eb9c-4f0d-aa26-25121ce8a893', N'Nước cốt dừa', N'Bơ sữa & Khác')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'67b156e3-039e-4987-bb6c-25a968212e76', N'Bún khô', N'Tinh bột')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'30cf0f2e-b16f-4490-bb5f-2637ef9f53b9', N'Hành tây', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'74f497e5-4868-4362-8afe-26644d1e3f3d', N'Bí ngòi', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'22415ddb-ba02-4f4a-8f45-281aca424c64', N'Bột chiên giòn', N'Tinh bột')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'6548d2cc-b479-4774-ad0e-2d597e9db54c', N'Phở khô', N'Tinh bột')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'2057d16d-55fa-48a8-b1c1-2ee52bc7e8fc', N'Gạo nếp', N'Tinh bột')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'3d81cc96-3a51-43c8-8aef-2ef7b5b4174c', N'Khoai lang', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'3e56c382-b050-4977-b6a5-2f93acafdbac', N'Bắp bò', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'144c6f56-de87-4435-89d9-32008da9f3eb', N'Đậu xanh', N'Tinh bột')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'18ad5bab-99a1-4075-87c5-3e6397225183', N'Giấm gạo', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'72e05543-7b9d-42ab-a053-40df0e6573db', N'Nước tương (Xì dầu)', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'dc65539a-c3e2-4d40-a068-43226a8c0001', N'Cà chua', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'deebfa7d-bfee-4f5c-a0b0-447681f69d2d', N'Cá hồi phi lê', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'218bf672-0c1e-47fc-a487-4772696414bc', N'Bột chiên xù', N'Tinh bột')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'f5dff3eb-f444-4b38-8c81-48680795bfd4', N'Tiêu đen', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'cd073170-bd5e-46f2-8a0b-4af8db520b45', N'Phô mai Mozzarella', N'Bơ sữa & Khác')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'e1b41198-247d-4720-8da9-4ef7812bcc7a', N'Ức gà', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'71f3b5ae-7218-4b02-a1f6-502a454d0f5a', N'Cánh gà', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'61541813-c2c8-4ef2-828e-53462e40626b', N'Bột matcha', N'Bơ sữa & Khác')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'59916531-904b-4405-ab46-54bb4005d4f6', N'Nước mắm', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'c92c1460-db00-40cb-bee1-556ff3d87300', N'Gạo lứt', N'Tinh bột')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'c307ed44-6e42-425f-bc78-566a5eaeac00', N'Tỏi', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'69eec91e-7983-4ce9-ae3b-58865a3cc98f', N'Thịt bò phi lê', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'a85ab1eb-e3e1-4476-82a5-5a9dbca5666b', N'Trứng cút', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'096815ad-9f30-4735-9f32-6044dfb05fb9', N'Sữa tươi có đường', N'Bơ sữa & Khác')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'b17cca03-cf20-4184-a5b2-623a0a2a06e9', N'Xúc xích', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'6fc23d65-910a-484f-b553-6466bb527565', N'Dầu ăn', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'35f25142-a2fc-4f2d-a4e6-681ab9aaea96', N'Bột ngọt (Mì chính)', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'e7165f78-67a3-433a-a244-6c4c97040c55', N'Bánh mì', N'Tinh bột')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'1c1c495e-3e54-44c0-9f1a-6cef757d1df1', N'Chanh', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'537c1a36-27fe-4d84-80af-6ee21a999715', N'Đường phèn', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'e7b2c3e6-1ffd-48e8-9450-6f1733a2a3a7', N'Dưa leo (Dưa chuột)', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'735e2df9-e052-45e7-92f7-770da89fe700', N'Ngũ vị hương', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'431798b7-aca5-4784-b184-777802dbf490', N'Yến mạch', N'Tinh bột')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'ba8c7fcb-45ed-49ce-b197-7b08ec0f69ca', N'Súp lơ xanh (Broccoli)', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'5c6f8f6f-0410-4856-a8b8-80e75802141c', N'Sườn non heo', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'3aba7b4e-3bb2-408d-9e0f-84a627b18c8b', N'Cà tím', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'6662b3b7-f86d-4c6b-b3ca-84f9bc140b35', N'Gạo trắng', N'Tinh bột')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'335c926e-adb2-4580-a255-85432f548c23', N'Thịt heo lợn xay', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'afe8227a-387b-4de8-9439-89c765fc7107', N'Mực ống', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'bdee5cef-09fa-472d-96bc-8c5269b30ba0', N'Ngò rí (Rau mùi)', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'4d34394c-081c-4e0e-8ba3-8d3ae4ea6ea2', N'Dầu hào', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'e1609bbe-ddb4-4265-ab4b-8f2561f17938', N'Dầu mè', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'8df8daaa-a3f3-48f2-95f8-8fd84d75d017', N'Súp lơ trắng', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'c7fbffdd-ea75-45d0-9460-92db35b435b7', N'Tương cà', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'92eb3491-e9f6-49fe-a355-92e342f5a2f9', N'Bột mì', N'Tinh bột')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'8a0ae828-0010-42dc-920f-935179b63a71', N'Xà lách', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'1e402ea2-e9b6-4071-82e9-95763d74c102', N'Cá lóc (Cá quả)', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'60e69f39-e8b6-447d-acfd-973202d42d9b', N'Ớt chuông', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'353c29d0-4916-4331-b8c1-991128765a3e', N'Trứng gà', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'bab7977d-be8b-4fc8-a848-9accfbc9781c', N'Đậu cô ve', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'96249faa-7f6d-4153-9ccd-9c28a46c60a5', N'Bắp ngọt (Ngô)', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'8fbfe033-3572-441b-910d-9c77b809e839', N'Thịt xông khói (Bacon)', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'993eaaa7-1c8f-4cf4-9a62-9d11d87c0f5d', N'Bí đỏ', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'37bc03b1-7798-4ed4-bad5-a037539fee32', N'Cá basa', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'f03bb047-05ce-40fd-8daa-a8fee0a03161', N'Khoai tây', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'01b838a8-6277-49b0-9eb5-a92bc6966ea9', N'Cà rốt', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'528bf2e7-8376-476d-b288-ab5645416009', N'Cải ngọt', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'fba95986-9695-462e-9418-abdb020ee0a5', N'Bột cacao', N'Bơ sữa & Khác')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'91bb47d5-a98d-49a0-8f9a-b43bd6da0230', N'Tía tô', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'd2851081-3cc9-481f-864e-b5904a4e3dbf', N'Hạt nêm', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'042fcc26-d0bf-429b-be94-b669ba480a20', N'Bột nghệ', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'503b4ea0-7f6d-4959-bc6e-b9e40d59afde', N'Bơ lạt', N'Bơ sữa & Khác')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'9e4d05a2-e5c8-41b7-9a6e-b9ef4139de79', N'Cải thìa (Cải chíp)', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'4aed809c-07a5-4f3d-be6f-ba68f1b738ff', N'Nấm đùi gà', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'8722a349-f2f7-401c-b7e8-bb5d00c96e8d', N'Hành tím', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'12dbb0b9-e5c0-43a7-8195-befbd6e26e2a', N'Rau muống', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'a5dce24f-31a0-44b2-8f8b-bf4074528f06', N'Bắp cải', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'8e4db7e3-e1b6-467e-a1f0-bf7368b6972e', N'Đậu đen', N'Tinh bột')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'c6b24e6c-4ebb-4b6d-9f13-c1792bfc9ed4', N'Kem tươi (Whipping cream)', N'Bơ sữa & Khác')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'474b8d1f-18f1-4bbe-953c-c1d569d3cd7b', N'Sữa tươi không đường', N'Bơ sữa & Khác')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'b158af36-0fad-4547-b86a-c56b72bf3e6e', N'Nấm hương', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'c3228950-2dce-4e64-add1-c5b477224655', N'Đường trắng', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'5c8f5c7c-0ce3-4cdc-800c-c78a16bdf740', N'Quất (Tắc)', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'3cb0a168-ce0f-478b-a127-c9b2f4451eca', N'Muối tinh', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'c4158690-c603-4a5e-bbb4-d302c9ce3b2c', N'Lạp xưởng', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'5c8e17b5-8056-4faf-b93b-d78fd7138401', N'Dầu oliu', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'eb743649-c574-4327-b260-db35584fcac3', N'Tôm sú', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'18351a00-3088-4b3f-9132-dd404c6a0eec', N'Gừng', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'29211684-8da1-4639-84fb-e528cd7dd6b8', N'Cải thảo', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'cbaa02be-94bf-4edf-b79a-e5a82cbe5da7', N'Tương ớt', N'Gia vị')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'690bab63-ccc8-4f8d-976d-e8e3ff885a1d', N'Bột bắp', N'Tinh bột')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'598e446f-939f-4372-937e-e9ed47702c63', N'Hành lá', N'Rau củ quả')
GO
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'8e9f00a2-b003-444b-967e-ebc7a6d42937', N'Miến dong', N'Tinh bột')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'447b432b-3324-446e-8eea-edc826ad471e', N'Khoai môn', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'14c77605-a4c9-40de-8873-f07a6bc6ea3e', N'Nấm kim châm', N'Rau củ quả')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'ca94c59f-692f-4703-bcc3-f0c5d7518be4', N'Bột năng', N'Tinh bột')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'8b1cc7b4-d992-402d-a2c5-fe2116c4fdf1', N'Thịt bò xay', N'Thịt & Hải sản')
INSERT [dbo].[StandardIngredients] ([Id], [Name], [Category]) VALUES (N'64d03348-5a33-4ace-b650-ffb05053c610', N'Gà nguyên con', N'Thịt & Hải sản')
GO
INSERT [dbo].[SuggestionRequests] ([Id], [UserId], [TargetBudgetAmount], [TargetBudgetCurrency], [DietaryRequirement], [AvailableToolsJson], [CreatedAt]) VALUES (N'1fd1fe56-43d4-4e54-94c3-549a12cc2d1c', N'5191a820-c43d-4a37-bf69-2ea5cfed4e69', CAST(1000000.00 AS Decimal(18, 2)), N'VND', N'None', N'[""]', CAST(N'2026-05-23T04:22:58.2628720' AS DateTime2))
GO
INSERT [dbo].[SuggestionResults] ([Id], [SuggestionRequestId], [DishCacheId], [TotalEstimatedPriceAmount], [TotalEstimatedPriceCurrency], [CreatedAt]) VALUES (N'aea7b5ec-7d33-4215-9179-1dc62b6b820a', N'1fd1fe56-43d4-4e54-94c3-549a12cc2d1c', N'9df22a44-952d-4623-a37e-2693cfac0464', CAST(0.00 AS Decimal(18, 2)), N'VND', CAST(N'2026-05-23T04:22:59.0163228' AS DateTime2))
INSERT [dbo].[SuggestionResults] ([Id], [SuggestionRequestId], [DishCacheId], [TotalEstimatedPriceAmount], [TotalEstimatedPriceCurrency], [CreatedAt]) VALUES (N'508a4632-6f5d-421c-8b59-2aa610c78a90', N'1fd1fe56-43d4-4e54-94c3-549a12cc2d1c', N'e757104a-daf3-425a-93eb-422b4d6151be', CAST(0.00 AS Decimal(18, 2)), N'VND', CAST(N'2026-05-23T04:22:58.7502681' AS DateTime2))
INSERT [dbo].[SuggestionResults] ([Id], [SuggestionRequestId], [DishCacheId], [TotalEstimatedPriceAmount], [TotalEstimatedPriceCurrency], [CreatedAt]) VALUES (N'a9ab5fd6-e391-48a6-8764-eb952eb9b69f', N'1fd1fe56-43d4-4e54-94c3-549a12cc2d1c', N'1beb8076-8a4e-4b65-bf8d-6d6d02f3b0ec', CAST(0.00 AS Decimal(18, 2)), N'VND', CAST(N'2026-05-23T04:22:59.0248274' AS DateTime2))
GO
INSERT [dbo].[UserHomepageCaches] ([Id], [UserId], [SerializedMenuData], [ExpirationTime], [CreatedAt]) VALUES (N'393ab46f-8235-4e59-843f-bace9a856294', N'5191a820-c43d-4a37-bf69-2ea5cfed4e69', N'{"IsPremiumExpired":true,"Breakfast":[],"Lunch":[],"Dinner":[{"DishName":"Asparagus and Pea Soup: Real Convenience Food","ImageUrl":"https://img.spoonacular.com/recipes/716406-312x231.jpg","Instructions":"","TotalCost":0,"Ingredients":[]},{"DishName":"Red Lentil Soup with Chicken and Turnips","ImageUrl":"https://img.spoonacular.com/recipes/715415-312x231.jpg","Instructions":"","TotalCost":0,"Ingredients":[]},{"DishName":"Garlicky Kale","ImageUrl":"https://img.spoonacular.com/recipes/644387-312x231.jpg","Instructions":"","TotalCost":0,"Ingredients":[]}]}', CAST(N'2026-05-24T04:25:59.4963864' AS DateTime2), CAST(N'2026-05-23T04:25:59.4540402' AS DateTime2))
GO
INSERT [dbo].[UserLogins] ([LoginProvider], [ProviderKey], [ProviderDisplayName], [UserId]) VALUES (N'Google', N'113999591533964420241', N'Google', N'5191a820-c43d-4a37-bf69-2ea5cfed4e69')
GO
INSERT [dbo].[Users] ([Id], [Username], [PasswordHash], [Email], [CreatedAt], [FullName], [IsActive], [RoleId]) VALUES (N'5191a820-c43d-4a37-bf69-2ea5cfed4e69', N'nguyenvana', N'$2a$12$fggCzKZcOsxFOdGIacUc5OUG0RpPJUeys6xrdp7Ohh0TMPoRQng72', N'satruong14@gmail.com', CAST(N'2026-05-14T02:01:11.4466667' AS DateTime2), NULL, 1, N'00000000-0000-0000-0000-000000000002')
INSERT [dbo].[Users] ([Id], [Username], [PasswordHash], [Email], [CreatedAt], [FullName], [IsActive], [RoleId]) VALUES (N'b4223454-c8bb-4a0d-a064-43873861fad2', N'', N'$2a$11$2kltI7.F98UBQazsOCAZRee25Gv0wPlZ6Q7SGPv9xV2nVlGsogW7G', NULL, CAST(N'2026-05-17T12:19:18.2346110' AS DateTime2), NULL, 1, N'00000000-0000-0000-0000-000000000002')
INSERT [dbo].[Users] ([Id], [Username], [PasswordHash], [Email], [CreatedAt], [FullName], [IsActive], [RoleId]) VALUES (N'e22dcd8c-698b-4f0d-be2c-b830e30f485e', N'admin', N'$2a$11$xhY8wyHlDMKNt8ka7UiuiuBoov5MlbDW1klW9SwE.h3qS1BoFHbNe', N'admin@fookit.com', CAST(N'2026-05-27T03:26:30.3400000' AS DateTime2), N'System Admin', 1, N'00000000-0000-0000-0000-000000000001')
GO
/****** Object:  Index [IX_AffiliateProducts_StandardIngredientId_IsActive]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE NONCLUSTERED INDEX [IX_AffiliateProducts_StandardIngredientId_IsActive] ON [dbo].[AffiliateProducts]
(
	[StandardIngredientId] ASC,
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IngredientDictionaries_StandardIngredientId]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE NONCLUSTERED INDEX [IX_IngredientDictionaries_StandardIngredientId] ON [dbo].[IngredientDictionaries]
(
	[StandardIngredientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Payments_SubscriptionPlanId]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE NONCLUSTERED INDEX [IX_Payments_SubscriptionPlanId] ON [dbo].[Payments]
(
	[SubscriptionPlanId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Payments_TransactionRef]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Payments_TransactionRef] ON [dbo].[Payments]
(
	[TransactionRef] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Payments_UserId]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE NONCLUSTERED INDEX [IX_Payments_UserId] ON [dbo].[Payments]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RefreshTokens_UserId]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE NONCLUSTERED INDEX [IX_RefreshTokens_UserId] ON [dbo].[RefreshTokens]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Roles_Name]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Roles_Name] ON [dbo].[Roles]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SuggestionRequests_UserId]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE NONCLUSTERED INDEX [IX_SuggestionRequests_UserId] ON [dbo].[SuggestionRequests]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SuggestionResults_DishCacheId]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE NONCLUSTERED INDEX [IX_SuggestionResults_DishCacheId] ON [dbo].[SuggestionResults]
(
	[DishCacheId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SuggestionResults_SuggestionRequestId]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE NONCLUSTERED INDEX [IX_SuggestionResults_SuggestionRequestId] ON [dbo].[SuggestionResults]
(
	[SuggestionRequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserAllergies_UserId]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE NONCLUSTERED INDEX [IX_UserAllergies_UserId] ON [dbo].[UserAllergies]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserDietaryPreferences_UserId]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE NONCLUSTERED INDEX [IX_UserDietaryPreferences_UserId] ON [dbo].[UserDietaryPreferences]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserHistories_UserId]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE NONCLUSTERED INDEX [IX_UserHistories_UserId] ON [dbo].[UserHistories]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserHomepageCaches_UserId]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE NONCLUSTERED INDEX [IX_UserHomepageCaches_UserId] ON [dbo].[UserHomepageCaches]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserLogins_UserId]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE NONCLUSTERED INDEX [IX_UserLogins_UserId] ON [dbo].[UserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Users_Email]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users]
(
	[Email] ASC
)
WHERE ([Email] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Users_RoleId]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE NONCLUSTERED INDEX [IX_Users_RoleId] ON [dbo].[Users]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Users_Username]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Username] ON [dbo].[Users]
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserSubscriptions_PlanId]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE NONCLUSTERED INDEX [IX_UserSubscriptions_PlanId] ON [dbo].[UserSubscriptions]
(
	[PlanId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserSubscriptions_UserId]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE NONCLUSTERED INDEX [IX_UserSubscriptions_UserId] ON [dbo].[UserSubscriptions]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserTools_UserId]    Script Date: 27/05/2026 10:30:01 SA ******/
CREATE NONCLUSTERED INDEX [IX_UserTools_UserId] ON [dbo].[UserTools]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AffiliateProducts] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[Payments] ADD  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[SuggestionRequests] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SuggestionResults] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[UserSubscriptions] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[AffiliateProducts]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateProducts_StandardIngredients_StandardIngredientId] FOREIGN KEY([StandardIngredientId])
REFERENCES [dbo].[StandardIngredients] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AffiliateProducts] CHECK CONSTRAINT [FK_AffiliateProducts_StandardIngredients_StandardIngredientId]
GO
ALTER TABLE [dbo].[IngredientDictionaries]  WITH CHECK ADD  CONSTRAINT [FK_IngredientDictionaries_StandardIngredients_StandardIngredientId] FOREIGN KEY([StandardIngredientId])
REFERENCES [dbo].[StandardIngredients] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[IngredientDictionaries] CHECK CONSTRAINT [FK_IngredientDictionaries_StandardIngredients_StandardIngredientId]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_SubscriptionPlans_SubscriptionPlanId] FOREIGN KEY([SubscriptionPlanId])
REFERENCES [dbo].[SubscriptionPlans] ([Id])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_SubscriptionPlans_SubscriptionPlanId]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_Users_UserId]
GO
ALTER TABLE [dbo].[RefreshTokens]  WITH CHECK ADD  CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RefreshTokens] CHECK CONSTRAINT [FK_RefreshTokens_Users_UserId]
GO
ALTER TABLE [dbo].[SuggestionRequests]  WITH CHECK ADD  CONSTRAINT [FK_SuggestionRequests_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SuggestionRequests] CHECK CONSTRAINT [FK_SuggestionRequests_Users_UserId]
GO
ALTER TABLE [dbo].[SuggestionResults]  WITH CHECK ADD  CONSTRAINT [FK_SuggestionResults_DishCaches_DishCacheId] FOREIGN KEY([DishCacheId])
REFERENCES [dbo].[DishCaches] ([Id])
GO
ALTER TABLE [dbo].[SuggestionResults] CHECK CONSTRAINT [FK_SuggestionResults_DishCaches_DishCacheId]
GO
ALTER TABLE [dbo].[SuggestionResults]  WITH CHECK ADD  CONSTRAINT [FK_SuggestionResults_SuggestionRequests_SuggestionRequestId] FOREIGN KEY([SuggestionRequestId])
REFERENCES [dbo].[SuggestionRequests] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SuggestionResults] CHECK CONSTRAINT [FK_SuggestionResults_SuggestionRequests_SuggestionRequestId]
GO
ALTER TABLE [dbo].[UserAllergies]  WITH CHECK ADD  CONSTRAINT [FK_UserAllergies_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserAllergies] CHECK CONSTRAINT [FK_UserAllergies_Users_UserId]
GO
ALTER TABLE [dbo].[UserDietaryPreferences]  WITH CHECK ADD  CONSTRAINT [FK_UserDietaryPreferences_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserDietaryPreferences] CHECK CONSTRAINT [FK_UserDietaryPreferences_Users_UserId]
GO
ALTER TABLE [dbo].[UserHistories]  WITH CHECK ADD  CONSTRAINT [FK_UserHistories_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserHistories] CHECK CONSTRAINT [FK_UserHistories_Users_UserId]
GO
ALTER TABLE [dbo].[UserHomepageCaches]  WITH CHECK ADD  CONSTRAINT [FK_UserHomepageCaches_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserHomepageCaches] CHECK CONSTRAINT [FK_UserHomepageCaches_Users_UserId]
GO
ALTER TABLE [dbo].[UserLogins]  WITH CHECK ADD  CONSTRAINT [FK_UserLogins_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserLogins] CHECK CONSTRAINT [FK_UserLogins_Users_UserId]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Roles_RoleId]
GO
ALTER TABLE [dbo].[UserSubscriptions]  WITH CHECK ADD  CONSTRAINT [FK_UserSubscriptions_SubscriptionPlans_PlanId] FOREIGN KEY([PlanId])
REFERENCES [dbo].[SubscriptionPlans] ([Id])
GO
ALTER TABLE [dbo].[UserSubscriptions] CHECK CONSTRAINT [FK_UserSubscriptions_SubscriptionPlans_PlanId]
GO
ALTER TABLE [dbo].[UserSubscriptions]  WITH CHECK ADD  CONSTRAINT [FK_UserSubscriptions_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserSubscriptions] CHECK CONSTRAINT [FK_UserSubscriptions_Users_UserId]
GO
ALTER TABLE [dbo].[UserTools]  WITH CHECK ADD  CONSTRAINT [FK_UserTools_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserTools] CHECK CONSTRAINT [FK_UserTools_Users_UserId]
GO
USE [master]
GO
ALTER DATABASE [FooKit] SET  READ_WRITE 
GO
