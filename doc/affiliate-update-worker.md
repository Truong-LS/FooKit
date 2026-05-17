# Tài Liệu Thiết Kế: Auto-Search Affiliate Worker (Real API & Data Lifecycle)

**Mục tiêu:** Xây dựng một Background Service trong .NET Core, quét danh sách `StandardIngredient`, gọi trực tiếp API Datafeed thật của Accesstrade để lấy link Affiliate mới nhất trên Shopee. Kết hợp cơ chế "Xóa mềm" (Soft Delete) để quản lý vòng đời dữ liệu, đảm bảo Database luôn sạch và AI có dữ liệu giá đa dạng.

---

## 1. Yêu Cầu Cập Nhật Database Entity
* Bổ sung thuộc tính `IsActive` (kiểu Boolean, mặc định = `true`) vào entity `AffiliateProduct`.
* Luật truy vấn: Mọi Service khác khi lấy dữ liệu sản phẩm để gợi ý món ăn chỉ được phép lấy các bản ghi có `IsActive == true`.

## 2. Vị Trí Kiến Trúc & Công Nghệ
* **Core:** `BackgroundService` kết hợp `PeriodicTimer`.
* **Database Access:** Khởi tạo `DbContext` thông qua `IServiceScopeFactory` (tránh rò rỉ bộ nhớ).
* **HTTP & Resilience:** Sử dụng `IHttpClientFactory` kết hợp thư viện **Polly** (Exponential Backoff Retry) để xử lý lỗi mạng.

## 3. Quản Lý Cấu Hình (Configuration)
Tạo class `AffiliateWorkerOptions` đọc từ IConfiguration. Phân tách rõ nơi lưu trữ:
* **Đọc từ `appsettings.json` (Không nhạy cảm):**
    * `SearchApiEndpoint`: `https://api.accesstrade.vn/v1/datafeeds`
    * `IntervalHours`: Chu kỳ chạy (VD: 12).
    * `BatchSize`: Số lượng nguyên liệu xử lý mỗi đợt (VD: 20).
    * `DelayBetweenRequestsMs`: Thời gian nghỉ giữa các lần gọi API (VD: 2000).
    * `MaxActiveLinksPerIngredient`: Giới hạn link active (VD: 3).
* **Đọc từ User Secrets / Environment Variables (Nhạy cảm):**
    * `AccessKey`: Token xác thực của Accesstrade.

## 4. Đặc Tả Giao Tiếp API Accesstrade Thật
Module `HttpClient` cần tuân thủ nghiêm ngặt các tiêu chuẩn sau:
* **Method:** `GET`
* **Headers:** Bắt buộc có key `Authorization` với giá trị `Token {AccessKey_Từ_Cấu_Hình}` (Lưu ý chữ Token có viết hoa và khoảng trắng).
* **Query Parameters:**
    * `keyword`: Tên nguyên liệu (BẮT BUỘC phải URL Encode).
    * `merchant`: Fix cứng là `shopee`.
    * `limit`: Fix cứng là `5` (Chỉ lấy 5 kết quả đầu tiên để tối ưu).
* **Cấu trúc DTO (Data Transfer Object):** Đón dữ liệu JSON trả về chuẩn `snake_case`. Cần map các trường: `total`, mảng `data` (chứa `product_id`, `name`, `price`, `affiliate_url`, `merchant`).

## 5. Luồng Thực Thi Chi Tiết (Nghiệp Vụ Lõi)

### Bước 1: Quét Nguyên Liệu (Fetch Target Ingredients)
* Mở `IServiceScope`.
* Truy vấn lấy ra `BatchSize` các `StandardIngredient` cần làm mới dữ liệu (Ưu tiên nguyên liệu có số link Active < MaxActiveLinks, hoặc link Active đã quá 24h).

### Bước 2 & 3: Gọi API & Phân Tích (Fetch & Parse Real Data)
* Truyền tên nguyên liệu vào Service xử lý gọi HTTP GET đến Accesstrade.
* Áp dụng Polly tự động thử lại tối đa 3 lần nếu HTTP Status Code là 5xx hoặc 429.
* Giải mã JSON. Chọn ra 1 sản phẩm có giá trị sử dụng tốt nhất (Giá > 0, có affiliate_url hợp lệ, chưa từng tồn tại URL này trong DB).

### Bước 4: Thêm Mới & Quản Lý Vòng Đời (Insert & Soft Delete)
* **Insert:** Tạo object `AffiliateProduct` mới từ dữ liệu thật. Gán `IsActive = true`, `LastUpdatedPriceAt = DateTime.UtcNow`. Gọi `.Add()`.
* **Soft Delete:** 1. Truy vấn các link hiện đang Active của nguyên liệu này, sắp xếp theo thời gian cập nhật giảm dần.
    2. Nếu tổng số lượng > `MaxActiveLinksPerIngredient` (ví dụ > 3), lấy các bản ghi cũ nhất bị dôi ra.
    3. Đổi `IsActive = false` cho các bản ghi dôi ra đó.

### Bước 5: Lưu và Nghỉ (Save & Delay)
* Gọi `SaveChangesAsync()`.
* `Task.Delay` theo cấu hình để tránh bị sàn chặn IP. Tiếp tục chu kỳ.

---

## 6. Lời Nhắc Prompt Dành Cho AI Agent (Dùng để gen code)

> "Tôi đang xây dựng dự án .NET Core Clean Architecture với EF Core. Dựa vào tài liệu thiết kế trên, hãy tạo toàn bộ mã nguồn liên quan. Yêu cầu TUYỆT ĐỐI KHÔNG DÙNG MOCK DATA.
> 
> **Chi tiết công việc:**
> 1. Viết class cấu hình `AffiliateWorkerOptions`.
> 2. Cập nhật entity `AffiliateProduct` thêm trường `IsActive`.
> 3. Định nghĩa các class DTO với `[JsonPropertyName]` để map chuẩn xác JSON trả về từ Accesstrade Datafeed API.
> 4. Viết Service gọi API thực tế qua `HttpClient`, xử lý mã hóa URL, gắn Header `Authorization: Token...` và map dữ liệu về DTO.
> 5. Viết class `AutoSearchAffiliateWorker` kế thừa `BackgroundService`. Bắt buộc dùng `IServiceScopeFactory`. Cài đặt nghiệp vụ lấy dữ liệu thật, Insert dòng mới và Soft Delete (set `IsActive = false`) các dòng cũ nếu vượt quá `MaxActiveLinks`.
> 6. Viết đoạn code đăng ký Dependency Injection cho `HttpClient` có đính kèm Polly Retry Policy (Exponential Backoff).
> 
> Hãy xuất ra các file `.cs` riêng biệt, tuân thủ SOLID, và có log chi tiết tiến trình."