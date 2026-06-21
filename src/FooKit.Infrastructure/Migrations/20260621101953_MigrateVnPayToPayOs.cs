using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FooKit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrateVnPayToPayOs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OrderCode",
                table: "Payments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "PayOsTransactionRef",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentLinkId",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            // Chuyển đổi dữ liệu cũ để tránh lỗi unique index = 0
            migrationBuilder.Sql(@"
                -- Cố gắng chuyển TransactionRef cũ thành OrderCode (nếu là số)
                UPDATE Payments
                SET OrderCode = TRY_CAST(TransactionRef AS BIGINT)
                WHERE TRY_CAST(TransactionRef AS BIGINT) IS NOT NULL;

                -- Nếu TransactionRef cũ không phải số, tạo mã giả ngẫu nhiên bằng ROW_NUMBER
                WITH CTE AS (
                    SELECT Id, ROW_NUMBER() OVER(ORDER BY CreatedAt) + 9999000000 AS NewOrderCode
                    FROM Payments
                    WHERE OrderCode = 0
                )
                UPDATE p
                SET p.OrderCode = c.NewOrderCode
                FROM Payments p
                INNER JOIN CTE c ON p.Id = c.Id;

                -- Copy mã giao dịch cũ
                UPDATE Payments
                SET PayOsTransactionRef = VnPayTransactionNo
                WHERE VnPayTransactionNo IS NOT NULL;
            ");

            migrationBuilder.DropIndex(
                name: "IX_Payments_TransactionRef",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TransactionRef",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "VnPayResponseCode",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "VnPayTransactionNo",
                table: "Payments");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderCode",
                table: "Payments",
                column: "OrderCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderCode",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OrderCode",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PayOsTransactionRef",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentLinkId",
                table: "Payments");

            migrationBuilder.AddColumn<string>(
                name: "TransactionRef",
                table: "Payments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VnPayResponseCode",
                table: "Payments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VnPayTransactionNo",
                table: "Payments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionRef",
                table: "Payments",
                column: "TransactionRef",
                unique: true);
        }
    }
}
