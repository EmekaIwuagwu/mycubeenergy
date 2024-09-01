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

            return dailyLimits.Select(dl => new UsageLimitDTO
            {
                Date = dl.Date,
                DailyLimit = dl.DailyTotalCost
            });
        }

        public async Task SaveDailyLimitsAsync(DailyLimit dailyLimit)
        {
            _context.DailyLimits.Add(dailyLimit);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalCostCountAsync(string email)
        {
            return await _context.Limits.CountAsync(l => l.Email == email);
        }

        public async Task SaveDailyLimitAsync(DailyLimit dailyLimit)
        {
            _context.DailyLimits.Add(dailyLimit);
            await _context.SaveChangesAsync();
        }

        public async Task<CashWallet> GetCashWalletByEmailAsync(string email)
        {
            return await _context.CashWallets.FirstOrDefaultAsync(cw => cw.Email == email);
        }

        public async Task UpdateCashWalletAsync(string email, decimal amount, string accountId, string transactionType)
        {
            var cashWallet = await _context.CashWallets.FirstOrDefaultAsync(cw => cw.Email == email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);

            if (cashWallet == null || user == null)
            {
                throw new Exception("User or Cash Wallet not found");
            }

            if (transactionType == "CREDIT")
            {
                cashWallet.Balance += amount;
                user.UnitBalance += amount;
            }
            else if (transactionType == "DEBIT")
            {
                cashWallet.Balance -= amount;
                user.UnitBalance -= amount;
            }
            else
            {
                throw new Exception("Invalid transaction type");
            }

            _context.CashWallets.Update(cashWallet);
            _context.Users.Update(user);

            // Log the transaction
            var transaction = new Transaction
            {
                Email = email,
                AccountId = accountId,
                Amount = amount,
                TransactionType = transactionType,
                TransactionDate = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();
        }


        public async Task LogTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<(decimal cashWalletBalance, decimal userWalletBalance)> DebitCashWalletAndCreditUserAsync(string email, decimal amount, string accountId)
        {
            var cashWallet = await _context.CashWallets.FirstOrDefaultAsync(cw => cw.Email == email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);

            if (cashWallet == null || user == null)
            {
                throw new Exception("User or Cash Wallet not found");
            }

            if (cashWallet.Balance < amount)
            {
                throw new Exception("Insufficient funds in Cash Wallet");
            }

            // Debit Cash Wallet
            cashWallet.Balance -= amount;

            // Credit User Wallet (assuming UnitBalance is the user wallet)
            user.UnitBalance += amount;

            // Log the transaction
            var transaction = new Transaction
            {
                Email = email,
                AccountId = accountId,
                Amount = -amount,  // Negative for debit
                TransactionType = "DEBIT",
                TransactionDate = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);

            // Save changes
            await _context.SaveChangesAsync();

            return (cashWallet.Balance, user.UnitBalance);
        }
    }
}
