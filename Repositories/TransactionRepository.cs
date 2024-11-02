using CubeEnergy.Data;
using CubeEnergy.Models;
using Microsoft.EntityFrameworkCore;

namespace CubeEnergy.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(string email, DateTime from, DateTime to)
        {
            return await _context.Transactions.Where(t => t.Email == email && t.CreatedAt >= from && t.CreatedAt <= to).ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByEmailAsync(string email)
        {
            return await _context.Transactions.Where(t => t.Email == email).ToListAsync();
        }

        public async Task LogTransactionAsync(Transaction transaction) // Change return type to Task
        {
            // Add the transaction to the context
            await _context.Transactions.AddAsync(transaction);
            // Save changes to the database
            await _context.SaveChangesAsync();
        }
    }
}
