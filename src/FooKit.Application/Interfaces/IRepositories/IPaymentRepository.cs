using FooKit.Domain.Entities;

namespace FooKit.Application.Interfaces.IRepositories
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        /// <summary>
        /// Finds a payment by its unique VNPay transaction reference.
        /// </summary>
        Task<Payment?> GetByTransactionRefAsync(string transactionRef);
        Task<IEnumerable<Payment>> GetByUserIdAsync(Guid userId);
    }
}
