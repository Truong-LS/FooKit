using Microsoft.EntityFrameworkCore;
using MyProject.Application.Interfaces.IRepositories;
using MyProject.Domain.Entities;
using MyProject.Infrastructure.Data.DBContext;

namespace MyProject.Infrastructure.Repositories
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
    }
}
