using CubeEnergy.Data;
using CubeEnergy.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CubeEnergy.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Admin> RegisterAdminAsync(Admin admin)
        {
            _context.AdminTables.Add(admin);
            await _context.SaveChangesAsync();
            return admin;
        }

        public async Task<Admin> GetAdminByUsernameAsync(string username)
        {
            return await _context.AdminTables.FirstOrDefaultAsync(a => a.AdminUsername == username);
        }

        public async Task<IEnumerable<Admin>> GetAllAdminsAsync()
        {
            return await _context.AdminTables.ToListAsync();
        }

        public async Task<Admin> GetAdminByIdAsync(int id)
        {
            return await _context.AdminTables.FindAsync(id);
        }

        public async Task UpdateAdminAsync(Admin admin)
        {
            _context.AdminTables.Update(admin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAdminAsync(int id)
        {
            var admin = await GetAdminByIdAsync(id);
            if (admin != null)
            {
                _context.AdminTables.Remove(admin);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserByAccountIdAsync(string accountId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
        }

        public async Task DeleteUserAsync(string accountId)
        {
            var user = await GetUserByAccountIdAsync(accountId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByEmailOrAccountIdAsync(string emailOrAccountId)
        {
            return await _context.Transactions
                .Where(t => t.Email == emailOrAccountId || t.AccountId == emailOrAccountId)
                .ToListAsync();
        }

        public async Task ReturnPaymentAsync(string debitAccountId, string creditAccountId, decimal amount)
        {
            // Find the user to be debited by accountId
            var debitUser = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == debitAccountId);
            if (debitUser == null)
            {
                throw new Exception("Debit User not found");
            }

            // Find the user to be credited by accountId
            var creditUser = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == creditAccountId);
            if (creditUser == null)
            {
                throw new Exception("Credit User not found");
            }

            // Ensure the debit user has sufficient balance
            if (debitUser.UnitBalance < amount)
            {
                throw new Exception("Insufficient balance for debit");
            }

            // Adjust balances
            debitUser.UnitBalance -= amount;
            creditUser.UnitBalance += amount;

            // Create a transaction entry for the debit
            var debitTransaction = new Transaction
            {
                AccountId = debitAccountId,
                Amount = -amount, // Negative amount for debit
                TransactionType = "Debit",
                TransactionDate = DateTime.UtcNow,
                Description = $"Refund to {creditAccountId}"
            };

            // Create a transaction entry for the credit
            var creditTransaction = new Transaction
            {
                AccountId = creditAccountId,
                Amount = amount, // Positive amount for credit
                TransactionType = "Credit",
                TransactionDate = DateTime.UtcNow,
                Description = $"Refund from {debitAccountId}"
            };

            // Add transactions to the Transactions table
            _context.Transactions.Add(debitTransaction);
            _context.Transactions.Add(creditTransaction);

            // Save changes to the database
            await _context.SaveChangesAsync();
        }

        public async Task<(int TotalUsers, decimal TotalTransactionAmount)> GetTransactionsAndTotalUsers()
        {
            int totalUsers = await _context.Users.CountAsync();
            decimal totalTransactionAmount = await _context.Transactions.SumAsync(t => t.Amount);

            return (totalUsers, totalTransactionAmount);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .OrderByDescending(o => o.Date) // Order by newest first
                .ToListAsync();
        }

        public async Task<Order> GetOrderByReferenceNumberAsync(string referenceNumber)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.ReferenceNumber == referenceNumber);
        }
    }
}
