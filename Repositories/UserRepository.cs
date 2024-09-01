using CubeEnergy.Data;
using CubeEnergy.DTOs;
using CubeEnergy.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CubeEnergy.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
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

        public async Task<UnitPrice> SaveUnitPriceAsync(UnitPrice price)
        {
            _context.UnitPrices.Add(price);
            await _context.SaveChangesAsync();
            return price;
        }

        public async Task<UnitPrice> UpdateUnitPriceAsync(UnitPrice price)
        {
            _context.UnitPrices.Update(price);
            await _context.SaveChangesAsync();
            return price;
        }

        public async Task DeleteUnitPriceAsync(int id)
        {
            var price = await _context.UnitPrices.FindAsync(id);
            _context.UnitPrices.Remove(price);
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

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == email);
        }

        public async Task<User> GetUserByAccountIdAsync(string accountId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
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

        public async Task UpdateCashWalletAsync(string email, decimal amount, string accountId, string transactionType)
        {
            var cashWallet = await _context.CashWallets.FirstOrDefaultAsync(cw => cw.Email == email);

            if (cashWallet != null)
            {
                if (transactionType == "CREDIT")
                {
                    cashWallet.Balance += amount;
                }
                else if (transactionType == "DEBIT")
                {
                    cashWallet.Balance -= amount;
                }

                var transaction = new Transaction
                {
                    Email = email,
                    Amount = amount,
                    TransactionDate = DateTime.UtcNow,
                    Description = "Cash Wallet Txn",
                    AccountId = accountId,
                    TransactionType = transactionType
                };

                _context.Transactions.Add(transaction);
                _context.CashWallets.Update(cashWallet);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Cash Wallet not found.");
            }
        }

        public async Task SaveDailyLimitsAsync(DailyLimit dailyLimit)
        {
            _context.DailyLimits.Add(dailyLimit);
            await _context.SaveChangesAsync();
        }

        public async Task SaveDailyLimitAsync(DailyLimit dailyLimit)
        {
            _context.DailyLimits.Update(dailyLimit);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalCostCountAsync(string email)
        {
            return await _context.DailyLimits
                .Where(dl => dl.Email == email)
                .CountAsync();
        }

        public async Task<CashWallet> GetCashWalletByEmailAsync(string email)
        {
            return await _context.CashWallets.FirstOrDefaultAsync(cw => cw.Email == email);
        }

        public async Task LogTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<(decimal cashWalletBalance, decimal userWalletBalance)> DebitCashWalletAndCreditUserAsync(string email, decimal amount, string accountId)
        {
            var user = await GetUserByEmailAsync(email);
            var cashWallet = await _context.CashWallets.FirstOrDefaultAsync(cw => cw.Email == email);
            var unitPrice = await _context.UnitPrices.FirstOrDefaultAsync(); // Assuming one price is applicable for all

            if (user == null || cashWallet == null || unitPrice == null)
                throw new Exception("User, Cash Wallet, or Unit Price not found.");

            if (cashWallet.Balance < amount)
                throw new Exception("Insufficient cash wallet balance.");

            // Calculate unit balance
            var unitBalance = amount * unitPrice.Price;

            cashWallet.Balance -= amount;
            user.UnitBalance += unitBalance;

            _context.CashWallets.Update(cashWallet);
            _context.Users.Update(user);

            var transaction = new Transaction
            {
                Email = email,
                Amount = amount,
                TransactionDate = DateTime.UtcNow,
                Description = "Cash Wallet Txn - Fund Wallet",
                AccountId = accountId,
                TransactionType = "DEBIT"
            };

            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();
            return (cashWallet.Balance, user.UnitBalance);
        }
    }
}
