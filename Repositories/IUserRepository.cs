﻿using CubeEnergy.DTOs;
using CubeEnergy.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CubeEnergy.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByAccountIdAsync(string accountId); // Add this method
        Task SaveDailyLimitsAsync(DailyLimit dailyLimit);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByEmailOrIdAsync(string email, int userId);
        Task<bool> DeleteUserAsync(int userId);
        Task UpdateUserAsync(User user);
        Task<UnitPrice> GetUnitPriceAsync(int id);
        Task<UnitPrice> GetUnitPriceAsync(); // Add if needed
        Task UpdateCashWalletAsync(string email, decimal amount, string accountId, string transactionType);
        Task<IEnumerable<Transaction>> GetTransactionsByEmailAsync(string email);
        Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(string email, DateTime from, DateTime to);
        Task<Result> ShareUnitsAsync(string originAccountId, string destinationAccountId, decimal amount);
        Task<decimal> CalculateTotalCostAsync(string email);
        Task<IEnumerable<UsageLimitDTO>> GetUsageLimitsByMonthAsync(string email, DateTime startDate, DateTime endDate);
        Task<(decimal CashWalletBalance, decimal UserWalletBalance)> DebitCashWalletAndCreditUserAsync(string email, decimal amount, string accountId);
        Task InsertCashWalletAndTransactionAsync(string email, decimal amount, string accountId, string transactionType);
    }

}
