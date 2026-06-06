using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Entities;

namespace FooKit.Infrastructure.ExternalServices
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(Payment payment, string ipAddress)
        {
            var tmnCode = _configuration["VNPAY_TMN_CODE"]!;
            var hashSecret = _configuration["VNPAY_HASH_SECRET"]!;
            var paymentUrl = _configuration["VNPAY_PAYMENT_URL"]!;
            var returnUrl = _configuration["VNPAY_RETURN_URL"]!;

            var vnpParams = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", tmnCode },
                { "vnp_Amount", ((long)(payment.Amount * 100)).ToString() },
                { "vnp_CurrCode", "VND" },
                { "vnp_TxnRef", payment.TransactionRef },
                { "vnp_OrderInfo", payment.OrderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_Locale", "vn" },
                { "vnp_ReturnUrl", returnUrl },
                { "vnp_IpAddr", ipAddress },
                { "vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss") },
                { "vnp_ExpireDate", DateTime.UtcNow.AddHours(7).AddMinutes(15).ToString("yyyyMMddHHmmss") }
            };

            // Build query string from sorted parameters
            var queryBuilder = new StringBuilder();
            foreach (var kvp in vnpParams)
            {
                if (queryBuilder.Length > 0)
                    queryBuilder.Append('&');

                queryBuilder.Append(WebUtility.UrlEncode(kvp.Key));
                queryBuilder.Append('=');
                queryBuilder.Append(WebUtility.UrlEncode(kvp.Value));
            }

            var signData = queryBuilder.ToString();
            var secureHash = HmacSha512(hashSecret, signData);

            return $"{paymentUrl}?{signData}&vnp_SecureHash={secureHash}";
        }

        public bool ValidateCallback(IDictionary<string, string> queryParams)
        {
            var hashSecret = _configuration["VNPAY_HASH_SECRET"]!;

            // Extract the secure hash from the callback
            if (!queryParams.TryGetValue("vnp_SecureHash", out var receivedHash))
                return false;

            // Build sorted parameter string excluding hash fields
            var sortedParams = new SortedDictionary<string, string>();
            foreach (var kvp in queryParams)
            {
                if (kvp.Key == "vnp_SecureHash" || kvp.Key == "vnp_SecureHashType")
                    continue;

                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    sortedParams.Add(kvp.Key, kvp.Value);
                }
            }

            var queryBuilder = new StringBuilder();
            foreach (var kvp in sortedParams)
            {
                if (queryBuilder.Length > 0)
                    queryBuilder.Append('&');

                queryBuilder.Append(WebUtility.UrlEncode(kvp.Key));
                queryBuilder.Append('=');
                queryBuilder.Append(WebUtility.UrlEncode(kvp.Value));
            }

            var computedHash = HmacSha512(hashSecret, queryBuilder.ToString());

            return string.Equals(computedHash, receivedHash, StringComparison.InvariantCultureIgnoreCase);
        }

        public string GetResponseCode(IDictionary<string, string> queryParams)
        {
            return queryParams.TryGetValue("vnp_ResponseCode", out var code) ? code : string.Empty;
        }

        /// <summary>
        /// Computes HMACSHA512 hash as required by VNPay signature specification.
        /// </summary>
        private static string HmacSha512(string key, string data)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
