﻿using CubeEnergy.DTOs;
using CubeEnergy.Models;

namespace CubeEnergy.Services
{
    public interface IUserService
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByEmailOrIdAsync(string email, int userId);
        Task<bool> DeleteUserAsync(int userId);
        Task UpdateUserAsync(User user);
        Task<decimal> GetUnitPriceAsync(int id);
        Task UpdateBalanceAndLogTransactionAsync(string email, decimal amount, string accountId, string transactionType);
        Task<IEnumerable<Transaction>> GetTransactionsByEmailAsync(string email);
        Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(string email, DateTime from, DateTime to);
        Task<Result> ShareUnitsAsync(string originAccountId, string destinationAccountId, decimal amount);
        Task<decimal> CalculateTotalCostAsync(string email);
        Task<IEnumerable<UsageLimit>> GetUsageLimitsByMonthAsync(string email, DateTime startDate, DateTime endDate);
        Task UpdateCashWalletAsync(string email, decimal amount, string accountId, string transactionType);
        Task<Balances> DebitCashWalletAndCreditUserAsync(string email, decimal amount, string accountId);
    }
}
