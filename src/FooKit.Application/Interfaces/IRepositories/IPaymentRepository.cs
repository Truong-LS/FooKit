using MyProject.Domain.Entities;

namespace MyProject.Application.Interfaces.IRepositories
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        /// <summary>
        /// Finds a payment by its unique VNPay transaction reference.
        /// </summary>
        Task<Payment?> GetByTransactionRefAsync(string transactionRef);
    }
}
