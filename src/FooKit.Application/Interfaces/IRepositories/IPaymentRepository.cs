using FooKit.Domain.Entities;

namespace FooKit.Application.Interfaces.IRepositories
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        /// <summary>
        /// Finds a payment by its unique PayOS order code.
        /// </summary>
        Task<Payment?> GetByOrderCodeAsync(long orderCode);
        Task<IEnumerable<Payment>> GetByUserIdAsync(Guid userId);
    }
}
