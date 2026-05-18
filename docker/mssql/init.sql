USE [master]
GO
/****** Object:  Database [FooKit]    Script Date: 18/05/2026 12:49:58 CH ******/
CREATE DATABASE [FooKit]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'FooKit', FILENAME = N'D:\FPTU\Tool\SQL\MSSQL16.MSSQLSERVER\MSSQL\DATA\FooKit.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'FooKit_log', FILENAME = N'D:\FPTU\Tool\SQL\MSSQL16.MSSQLSERVER\MSSQL\DATA\FooKit_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
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
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 18/05/2026 12:49:59 CH ******/
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
/****** Object:  Table [dbo].[AffiliateProducts]    Script Date: 18/05/2026 12:49:59 CH ******/
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
 CONSTRAINT [PK_AffiliateProducts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DishCaches]    Script Date: 18/05/2026 12:49:59 CH ******/
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
/****** Object:  Table [dbo].[IngredientDictionaries]    Script Date: 18/05/2026 12:49:59 CH ******/
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
/****** Object:  Table [dbo].[Payments]    Script Date: 18/05/2026 12:49:59 CH ******/
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
/****** Object:  Table [dbo].[RefreshTokens]    Script Date: 18/05/2026 12:49:59 CH ******/
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
/****** Object:  Table [dbo].[StandardIngredients]    Script Date: 18/05/2026 12:49:59 CH ******/
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
/****** Object:  Table [dbo].[SubscriptionPlans]    Script Date: 18/05/2026 12:49:59 CH ******/
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
/****** Object:  Table [dbo].[SuggestionRequests]    Script Date: 18/05/2026 12:49:59 CH ******/
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
/****** Object:  Table [dbo].[SuggestionResults]    Script Date: 18/05/2026 12:49:59 CH ******/
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
/****** Object:  Table [dbo].[UserDietaryPreferences]    Script Date: 18/05/2026 12:49:59 CH ******/
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
/****** Object:  Table [dbo].[UserLogins]    Script Date: 18/05/2026 12:49:59 CH ******/
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
/****** Object:  Table [dbo].[Users]    Script Date: 18/05/2026 12:49:59 CH ******/
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
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserSubscriptions]    Script Date: 18/05/2026 12:49:59 CH ******/
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
/****** Object:  Table [dbo].[UserTools]    Script Date: 18/05/2026 12:49:59 CH ******/
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
/****** Object:  Index [IX_AffiliateProducts_StandardIngredientId]    Script Date: 18/05/2026 12:49:59 CH ******/
CREATE NONCLUSTERED INDEX [IX_AffiliateProducts_StandardIngredientId] ON [dbo].[AffiliateProducts]
(
	[StandardIngredientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_IngredientDictionaries_StandardIngredientId]    Script Date: 18/05/2026 12:49:59 CH ******/
CREATE NONCLUSTERED INDEX [IX_IngredientDictionaries_StandardIngredientId] ON [dbo].[IngredientDictionaries]
(
	[StandardIngredientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Payments_SubscriptionPlanId]    Script Date: 18/05/2026 12:49:59 CH ******/
CREATE NONCLUSTERED INDEX [IX_Payments_SubscriptionPlanId] ON [dbo].[Payments]
(
	[SubscriptionPlanId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Payments_TransactionRef]    Script Date: 18/05/2026 12:49:59 CH ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Payments_TransactionRef] ON [dbo].[Payments]
(
	[TransactionRef] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Payments_UserId]    Script Date: 18/05/2026 12:49:59 CH ******/
CREATE NONCLUSTERED INDEX [IX_Payments_UserId] ON [dbo].[Payments]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RefreshTokens_UserId]    Script Date: 18/05/2026 12:49:59 CH ******/
CREATE NONCLUSTERED INDEX [IX_RefreshTokens_UserId] ON [dbo].[RefreshTokens]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SuggestionRequests_UserId]    Script Date: 18/05/2026 12:49:59 CH ******/
CREATE NONCLUSTERED INDEX [IX_SuggestionRequests_UserId] ON [dbo].[SuggestionRequests]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SuggestionResults_DishCacheId]    Script Date: 18/05/2026 12:49:59 CH ******/
CREATE NONCLUSTERED INDEX [IX_SuggestionResults_DishCacheId] ON [dbo].[SuggestionResults]
(
	[DishCacheId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SuggestionResults_SuggestionRequestId]    Script Date: 18/05/2026 12:49:59 CH ******/
CREATE NONCLUSTERED INDEX [IX_SuggestionResults_SuggestionRequestId] ON [dbo].[SuggestionResults]
(
	[SuggestionRequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserDietaryPreferences_UserId]    Script Date: 18/05/2026 12:49:59 CH ******/
CREATE NONCLUSTERED INDEX [IX_UserDietaryPreferences_UserId] ON [dbo].[UserDietaryPreferences]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserLogins_UserId]    Script Date: 18/05/2026 12:49:59 CH ******/
CREATE NONCLUSTERED INDEX [IX_UserLogins_UserId] ON [dbo].[UserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Users_Email]    Script Date: 18/05/2026 12:49:59 CH ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users]
(
	[Email] ASC
)
WHERE ([Email] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Users_Username]    Script Date: 18/05/2026 12:49:59 CH ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Username] ON [dbo].[Users]
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserSubscriptions_PlanId]    Script Date: 18/05/2026 12:49:59 CH ******/
CREATE NONCLUSTERED INDEX [IX_UserSubscriptions_PlanId] ON [dbo].[UserSubscriptions]
(
	[PlanId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserSubscriptions_UserId]    Script Date: 18/05/2026 12:49:59 CH ******/
CREATE NONCLUSTERED INDEX [IX_UserSubscriptions_UserId] ON [dbo].[UserSubscriptions]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UserTools_UserId]    Script Date: 18/05/2026 12:49:59 CH ******/
CREATE NONCLUSTERED INDEX [IX_UserTools_UserId] ON [dbo].[UserTools]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
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
ALTER TABLE [dbo].[UserDietaryPreferences]  WITH CHECK ADD  CONSTRAINT [FK_UserDietaryPreferences_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserDietaryPreferences] CHECK CONSTRAINT [FK_UserDietaryPreferences_Users_UserId]
GO
ALTER TABLE [dbo].[UserLogins]  WITH CHECK ADD  CONSTRAINT [FK_UserLogins_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserLogins] CHECK CONSTRAINT [FK_UserLogins_Users_UserId]
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
