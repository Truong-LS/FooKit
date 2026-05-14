using MyProject.Domain.Entities;

namespace MyProject.Application.Interfaces.IServices
{
    /// <summary>
    /// Abstracts VNPay gateway operations. Implemented in Infrastructure layer.
    /// </summary>
    public interface IVnPayService
    {
        /// <summary>
        /// Generates the VNPay payment URL to redirect the user to the payment gateway.
        /// </summary>
        string CreatePaymentUrl(Payment payment, string ipAddress);

        /// <summary>
        /// Validates the VNPay callback query parameters by verifying the HMACSHA512 signature.
        /// Returns true if the signature is valid and data has not been tampered with.
        /// </summary>
        bool ValidateCallback(IDictionary<string, string> queryParams);

        /// <summary>
        /// Extracts the VNPay response code from callback query parameters.
        /// "00" indicates a successful transaction.
        /// </summary>
        string GetResponseCode(IDictionary<string, string> queryParams);
    }
}
