# Database Schema: AI-Powered Recipe & Affiliate System

## 1. Nhóm Quản lý Người dùng & Gói cước (Identity Context)
Quản lý thông tin tài khoản và phân quyền truy cập các tính năng Premium (như lọc theo chế độ ăn).

### Table: `User`
- `Id` (Guid) - Primary Key
- `FullName` (string)
- `Email` (string)
- `PasswordHash` (string)
- `CreatedAt` (DateTime)
- `IsActive` (bool)

### Table: `SubscriptionPlan`
- `Id` (Guid) - Primary Key
- `PlanName` (string) - VD: Free, Premium
- `Price` (decimal)
- `DurationInDays` (int)
- `FeaturesJson` (string) - JSON mô tả các tính năng của gói

### Table: `UserSubscription`
- `Id` (Guid) - Primary Key
- `UserId` (Guid) - Foreign Key -> `User`
- `PlanId` (Guid) - Foreign Key -> `SubscriptionPlan`
- `StartDate` (DateTime)
- `EndDate` (DateTime)
- `IsActive` (bool)

---

## 2. Nhóm Cá nhân hóa (Profile Context)
Lưu trữ các thiết lập mặc định của người dùng để làm đầu vào cho AI.

### Table: `UserDietaryPreference`
- `Id` (Guid) - Primary Key
- `UserId` (Guid) - Foreign Key -> `User`
- `DietaryType` (string) - VD: Vegan, Keto, Eat Clean

### Table: `UserTool`
- `Id` (Guid) - Primary Key
- `UserId` (Guid) - Foreign Key -> `User`
- `ToolName` (string) - VD: Lò nướng, Nồi chiên không dầu, Máy xay sinh tố

---

## 3. Nhóm Bộ nhớ đệm & Dữ liệu ngoại vi (Cache & External Context)
Phục vụ luồng On-the-fly, lưu tạm dữ liệu từ 3rd Party API để giảm tải request và tối ưu tốc độ.

### Table: `DishCache`
- `Id` (Guid) - Primary Key
- `ExternalApiId` (string) - ID món ăn từ Spoonacular/Edamam
- `Name` (string) - Tên món ăn
- `ImageUrl` (string) - Link ảnh món ăn
- `DietaryTagsJson` (string) - JSON mảng các tag chế độ ăn
- `RequiredToolsJson` (string) - JSON mảng các dụng cụ cần thiết
- `RawIngredientsJson` (string) - JSON nguyên liệu gốc từ API (VD: `["2 chicken breasts", "1 tsp salt"]`)
- `LastFetchedAt` (DateTime) - Thời gian đồng bộ cuối cùng

### Table: `IngredientDictionary`
- `Id` (Guid) - Primary Key
- `RawKeywordFromApi` (string) - Text thô do AI bóc tách (VD: "boneless chicken breast")
- `StandardIngredientId` (Guid) - Foreign Key -> `StandardIngredient`

---

## 4. Nhóm Nguyên liệu & Tiếp thị liên kết (Commercial Context)
Quản lý nguyên liệu chuẩn và các sản phẩm Affiliate thực tế trên các sàn TMĐT.

### Table: `StandardIngredient`
- `Id` (Guid) - Primary Key
- `Name` (string) - Tên nguyên liệu chuẩn (VD: "Ức gà", "Muối")
- `Category` (string) - Phân loại (VD: Thịt, Gia vị, Rau củ)

### Table: `AffiliateProduct`
- `Id` (Guid) - Primary Key
- `StandardIngredientId` (Guid) - Foreign Key -> `StandardIngredient`
- `ProductName` (string) - Tên sản phẩm trên sàn
- `ProductUrl` (string) - Link Affiliate tracking
- `CurrentPrice` (decimal) - Giá tiền để AI tính toán tổng chi phí
- `Platform` (string) - VD: Shopee, TikTok, Lazada
- `LastUpdatedPriceAt` (DateTime) - Cập nhật giá lần cuối

---

## 5. Nhóm AI & Lịch sử gợi ý (AI Orchestration Context)
Lưu trữ "đề bài" của user và kết quả trả về từ hệ thống (kết hợp API + AI).

### Table: `SuggestionRequest`
- `Id` (Guid) - Primary Key
- `UserId` (Guid) - Foreign Key -> `User`
- `TargetBudget` (decimal) - Giá tiền tối đa user yêu cầu
- `DietaryRequirement` (string) - Yêu cầu chế độ ăn (nếu có)
- `AvailableToolsJson` (string) - JSON danh sách dụng cụ user có tại thời điểm đó
- `CreatedAt` (DateTime)

### Table: `SuggestionResult`
- `Id` (Guid) - Primary Key
- `SuggestionRequestId` (Guid) - Foreign Key -> `SuggestionRequest`
- `DishCacheId` (Guid) - Foreign Key -> `DishCache`
- `TotalEstimatedPrice` (decimal) - Tổng giá tiền Affiliate thực tế AI đã tính toán ra
- `CreatedAt` (DateTime)

---

## 6. Mối Quan Hệ Giữa Các Bảng (Entity Relationships)

Cấu trúc database này được thiết kế chủ yếu dựa trên các mối quan hệ **Một - Nhiều (1:N)** để đảm bảo tính mở rộng.

* **Khối Người dùng & Mở rộng:**
  * `User` (1) - (N) `UserSubscription`: Một người dùng có thể có nhiều lịch sử gói cước.
  * `SubscriptionPlan` (1) - (N) `UserSubscription`: Một gói cước được đăng ký bởi nhiều người dùng.
  * `User` (1) - (N) `UserDietaryPreference`: Người dùng có thể thiết lập nhiều chế độ ăn.
  * `User` (1) - (N) `UserTool`: Người dùng có thể sở hữu nhiều loại dụng cụ bếp khác nhau.

* **Khối Trạm trung chuyển AI & Affiliate:**
  * `StandardIngredient` (1) - (N) `IngredientDictionary`: Một nguyên liệu chuẩn (VD: "Ức gà") được ánh xạ từ vô số từ khóa thô từ API (VD: "chicken breast", "ức gà không xương"). Đây là chìa khóa để AI hiểu data.
  * `StandardIngredient` (1) - (N) `AffiliateProduct`: Một nguyên liệu chuẩn có nhiều lựa chọn mua hàng (nhiều link Shopee, TikTok với các giá khác nhau).

* **Khối Lịch sử xử lý:**
  * `User` (1) - (N) `SuggestionRequest`: Một người dùng có thể yêu cầu AI gợi ý nhiều lần.
  * `SuggestionRequest` (1) - (N) `SuggestionResult`: Một lần yêu cầu, AI có thể trả về một danh sách gồm nhiều món ăn.
  * `DishCache` (1) - (N) `SuggestionResult`: Một món ăn đã cache có thể xuất hiện trong nhiều kết quả gợi ý của nhiều người dùng khác nhau.

### Sơ đồ ERD (Mermaid)
*(Nếu bạn paste đoạn code này vào file Markdown trên GitHub, GitLab, hoặc Notion, nó sẽ tự động render thành một bản đồ database trực quan)*

```mermaid
erDiagram
    User ||--o{ UserSubscription : has
    SubscriptionPlan ||--o{ UserSubscription : includes
    User ||--o{ UserDietaryPreference : prefers
    User ||--o{ UserTool : owns
    User ||--o{ SuggestionRequest : makes

    SuggestionRequest ||--o{ SuggestionResult : generates
    DishCache ||--o{ SuggestionResult : referenced_in

    StandardIngredient ||--o{ IngredientDictionary : mapped_by
    StandardIngredient ||--o{ AffiliateProduct : linked_to