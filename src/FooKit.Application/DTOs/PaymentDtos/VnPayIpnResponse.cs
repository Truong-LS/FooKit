namespace MyProject.Application.DTOs.PaymentDtos
{
    /// <summary>
    /// Response format required by VNPay IPN callback.
    /// VNPay expects exactly this JSON structure.
    /// </summary>
    public class VnPayIpnResponse
    {
        public string RspCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
