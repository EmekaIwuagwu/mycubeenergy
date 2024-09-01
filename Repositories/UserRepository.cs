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

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByEmailOrIdAsync(string email, int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email || u.Id == userId);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UnitPrice> GetUnitPriceAsync(int id)
        {
            return await _context.UnitPrices.FindAsync(id);
        }

        public async Task<UnitPrice> GetUnitPriceAsync()
        {
            return await _context.UnitPrices.FirstOrDefaultAsync(); // Add as required
        }

        /*public async Task UpdateCashWalletAsync(string email, decimal amount, string accountId, string transactionType)
        {
            var user = await GetUserByEmailAsync(email);
            if (user != null)
            {
                if (transactionType == "Credit")
                {
                    user.UnitBalance += amount;
                }
                else if (transactionType == "Debit")
                {
                    user.UnitBalance -= amount;
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }
        */

        public async Task UpdateCashWalletAsync(string email, decimal amount, string accountId, string transactionType)
        {
            var user = await GetUserByEmailAsync(email);
            if (user != null)
            {
                // Insert record into CashWallets table
                var cashWallet = new CashWallet
                {
                    Email = email,
                    Balance = transactionType == "Credit" ? user.UnitBalance + amount : user.UnitBalance - amount,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.CashWallets.AddAsync(cashWallet);

                // Save the transaction details into the Transactions table
                var transaction = new Transaction
                {
                    Email = email,
                    Amount = amount,
                    TransactionType = transactionType,
                    AccountId = accountId,
                    Description = "New Cash In Payment",
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Transactions.AddAsync(transaction);

                // Save changes to both tables
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByEmailAsync(string email)
        {
            return await _context.Transactions.Where(t => t.Email == email).ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(string email, DateTime from, DateTime to)
        {
            return await _context.Transactions.Where(t => t.Email == email && t.CreatedAt >= from && t.CreatedAt <= to).ToListAsync();
        }

        public async Task<Result> ShareUnitsAsync(string originAccountId, string destinationAccountId, decimal amount)
        {
            var originUser = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == originAccountId);
            var destinationUser = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == destinationAccountId);

            if (originUser != null && destinationUser != null)
            {
                if (originUser.UnitBalance >= amount)
                {
                    originUser.UnitBalance -= amount;
                    destinationUser.UnitBalance += amount;

                    _context.Users.Update(originUser);
                    _context.Users.Update(destinationUser);

                    await _context.SaveChangesAsync();

                    return new Result { Success = true };
                }

                return new Result { Success = false, ErrorMessage = "Insufficient balance." };
            }

            return new Result { Success = false, ErrorMessage = "User not found." };
        }

        public async Task<decimal> CalculateTotalCostAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);
            if (user != null)
            {
                var dailyLimits = await _context.DailyLimits.Where(dl => dl.UserId == user.Id).ToListAsync();
                return dailyLimits.Sum(dl => dl.Amount);
            }

            return 0;
        }

        public async Task<IEnumerable<UsageLimitDTO>> GetUsageLimitsByMonthAsync(string email, DateTime startDate, DateTime endDate)
        {
            var user = await GetUserByEmailAsync(email);
            if (user != null)
            {
                return await _context.UsageLimits
                    .Where(ul => ul.UserId == user.Id && ul.Date >= startDate && ul.Date <= endDate)
                    .Select(ul => new UsageLimitDTO
                    {
                        Date = ul.Date,
                        Limit = ul.Limit
                    })
                    .ToListAsync();
            }

            return Enumerable.Empty<UsageLimitDTO>();
        }

        public async Task<(decimal CashWalletBalance, decimal UserWalletBalance)> DebitCashWalletAndCreditUserAsync(string email, decimal amount, string accountId)
        {
            var user = await GetUserByEmailAsync(email);
            var unitPrice = await GetUnitPriceAsync(); // Adjust if needed

            if (user != null && unitPrice != null)
            {
                var totalCost = amount * unitPrice.Price;

                // Update cash wallet balance
                user.UnitBalance -= totalCost;

                // Update user balance
                var userWallet = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
                if (userWallet != null)
                {
                    userWallet.UnitBalance += amount;
                    _context.Users.Update(userWallet);
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return (user.UnitBalance, userWallet?.UnitBalance ?? 0);
            }

            return (0, 0);
        }

        public async Task<User> GetUserByAccountIdAsync(string accountId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
        }

        public async Task SaveDailyLimitsAsync(DailyLimit dailyLimit)
        {
            _context.DailyLimits.Add(dailyLimit);
            await _context.SaveChangesAsync();
        }

        public async Task InsertCashWalletAndTransactionAsync(string email, decimal amount, string accountId, string transactionType)
        {
            // Find the user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            // Update the user's unit balance based on the transaction type
            if (transactionType == "Credit")
            {
                user.UnitBalance += amount;
            }
            else if (transactionType == "Debit")
            {
                user.UnitBalance -= amount;
            }
            else
            {
                throw new ArgumentException("Invalid transaction type.");
            }

            // Update the Users table
            _context.Users.Update(user);

            // Add a new transaction entry to the Transactions table
            var transaction = new Transaction
            {
                Email = email,
                Amount = amount,
                AccountId = accountId,
                Description = "Wallet Txn",
                TransactionType = transactionType,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Transactions.AddAsync(transaction);

            // Update the CashWallet balance
            var cashWallet = await _context.CashWallets.FirstOrDefaultAsync(cw => cw.Email == email);

            if (cashWallet == null)
            {
                // If the user does not have a cash wallet record, create one
                cashWallet = new CashWallet
                {
                    Email = email,
                    Balance = transactionType == "Credit" ? amount : -amount,
                    AccountId = accountId,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.CashWallets.AddAsync(cashWallet);
            }
            else
            {
                // If the user has a cash wallet record, update the balance
                if (transactionType == "Credit")
                {
                    cashWallet.Balance += amount;
                }
                else if (transactionType == "Debit")
                {
                    cashWallet.Balance -= amount;
                }
                _context.CashWallets.Update(cashWallet);
            }

            // Save changes to both tables
            await _context.SaveChangesAsync();
        }


    }

}
