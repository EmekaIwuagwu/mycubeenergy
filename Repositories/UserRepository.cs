using CubeEnergy.Data;
using CubeEnergy.DTOs;
using CubeEnergy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CubeEnergy.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == email);
        }

        public async Task<User> GetUserByAccountIdAsync(string accountId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByEmailAsync(string email)
        {
            return await _context.Transactions.Where(t => t.Email == email).ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(string email, DateTime from, DateTime to)
        {
            return await _context.Transactions.Where(t => t.Email == email && t.TransactionDate >= from && t.TransactionDate <= to).ToListAsync();
        }

        public async Task<UnitPrice> GetUnitPriceAsync(int id)
        {
            return await _context.UnitPrices.FindAsync(id);
        }

        public async Task<bool> ShareUnitsAsync(string originAccountId, string destinationAccountId, decimal amount)
        {
            var originUser = await GetUserByAccountIdAsync(originAccountId);
            var destinationUser = await GetUserByAccountIdAsync(destinationAccountId);

            if (originUser == null || destinationUser == null || originUser.UnitBalance < amount)
                return false;

            originUser.UnitBalance -= amount;
            destinationUser.UnitBalance += amount;

            _context.Users.Update(originUser);
            _context.Users.Update(destinationUser);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<decimal> CalculateTotalCostAsync(string email)
        {
            var totalCost = await _context.DailyLimits
                .Where(dl => dl.Email == email)
                .SumAsync(dl => dl.DailyTotalCost);

            return totalCost;
        }

        public async Task<IEnumerable<UsageLimitDTO>> GetUsageLimitsByMonthAsync(string email, DateTime startDate, DateTime endDate)
        {
            var dailyLimits = await _context.DailyLimits
                .Where(dl => dl.Email == email && dl.Date >= startDate && dl.Date <= endDate)
                .ToListAsync();

            var usageLimits = dailyLimits
                .GroupBy(dl => new { dl.Date.Year, dl.Date.Month })
                .Select(g => new UsageLimitDTO
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:00}",
                    TotalUsage = g.Sum(dl => dl.DailyUsage)
                });

            return usageLimits;
        }

        public async Task<CashWallet> GetCashWalletByEmailAsync(string email)
        {
            return await _context.CashWallets.FirstOrDefaultAsync(cw => cw.Email == email);
        }

        public async Task UpdateCashWalletAsync(string email, decimal amount, string accountId, string transactionType)
        {
            _logger.LogInformation("Updating cash wallet. Email: {Email}, Amount: {Amount}, AccountId: {AccountId}, TransactionType: {TransactionType}",
                email, amount, accountId, transactionType);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == email);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            if (transactionType == "Credit")
            {
                user.UnitBalance += amount;
            }
            else if (transactionType == "Debit" && user.UnitBalance >= amount)
            {
                user.UnitBalance -= amount;
            }
            else
            {
                throw new ArgumentException("Invalid transaction type or insufficient balance.");
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Cash wallet updated successfully for Email: {Email}", email);
        }

        public async Task<(decimal cashWalletBalance, decimal userWalletBalance)> DebitCashWalletAndCreditUserAsync(string email, decimal amount, string accountId)
        {
            var cashWallet = await _context.CashWallets.FirstOrDefaultAsync(cw => cw.Email == email);
            var user = await GetUserByAccountIdAsync(accountId);

            if (cashWallet == null || user == null || cashWallet.Balance < amount)
            {
                throw new ArgumentException("Insufficient funds or user not found.");
            }

            cashWallet.Balance -= amount;
            user.UnitBalance += amount;

            _context.CashWallets.Update(cashWallet);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return (cashWallet.Balance, user.UnitBalance);
        }

        public async Task LogTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalCostCountAsync(string email)
        {
            return await _context.DailyLimits.Where(dl => dl.Email == email).CountAsync();
        }

        public async Task SaveDailyLimitsAsync(DailyLimit dailyLimit)
        {
            _context.DailyLimits.Add(dailyLimit);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUnitPriceAsync(int id)
        {
            var unitPrice = await _context.UnitPrices.FindAsync(id);
            if (unitPrice != null)
            {
                _context.UnitPrices.Remove(unitPrice);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UnitPrice> UpdateUnitPriceAsync(UnitPrice unitPrice)
        {
            _context.UnitPrices.Update(unitPrice);
            await _context.SaveChangesAsync();
            return unitPrice;
        }

        public async Task<UnitPrice> SaveUnitPriceAsync(UnitPrice unitPrice)
        {
            _context.UnitPrices.Add(unitPrice);
            await _context.SaveChangesAsync();
            return unitPrice;
        }

        public async Task SaveDailyLimitAsync(DailyLimit dailyLimit)
        {
            _context.DailyLimits.Add(dailyLimit);
            await _context.SaveChangesAsync();
        }

    }
}
