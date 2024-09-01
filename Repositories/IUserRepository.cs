using CubeEnergy.DTOs;
using CubeEnergy.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CubeEnergy.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int id);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<UnitPrice> SaveUnitPriceAsync(UnitPrice price);
        Task<UnitPrice> UpdateUnitPriceAsync(UnitPrice price);
        Task DeleteUnitPriceAsync(int id);
        Task<IEnumerable<Transaction>> GetTransactionsByEmailAsync(string email);
        Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(string email, DateTime from, DateTime to);
        Task<UnitPrice> GetUnitPriceAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByAccountIdAsync(string accountId);
        Task<bool> ShareUnitsAsync(string originAccountId, string destinationAccountId, decimal amount);
        Task<decimal> CalculateTotalCostAsync(string email);
        Task<IEnumerable<UsageLimitDTO>> GetUsageLimitsByMonthAsync(string email, DateTime startDate, DateTime endDate);
        Task SaveDailyLimitsAsync(DailyLimit dailyLimit);
        Task<int> GetTotalCostCountAsync(string email);

        // New Methods for Cash Wallet Management
        Task<CashWallet> GetCashWalletByEmailAsync(string email);
        Task UpdateCashWalletAsync(string email, decimal amount, string accountId, string transactionType);
        Task LogTransactionAsync(Transaction transaction);

        // New Method to Debit Cash Wallet and Credit User Wallet
        Task<(decimal cashWalletBalance, decimal userWalletBalance)> DebitCashWalletAndCreditUserAsync(string email, decimal amount, string accountId);
    }
}
