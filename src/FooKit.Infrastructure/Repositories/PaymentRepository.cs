using Microsoft.EntityFrameworkCore;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Domain.Entities;
using FooKit.Infrastructure.Data.DBContext;

namespace FooKit.Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(FooKitDbContext context) : base(context)
        {
        }

        public async Task<Payment?> GetByTransactionRefAsync(string transactionRef)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionRef == transactionRef);
        }

        public async Task<IEnumerable<Payment>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Payments
                .Include(x => x.SubscriptionPlan)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}
