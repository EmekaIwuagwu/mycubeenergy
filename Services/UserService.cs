using CubeEnergy.DTOs;
using CubeEnergy.Models;
using CubeEnergy.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CubeEnergy.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private const decimal BillsUsageRatePerHour = 10.50m;

        public UserService(IUserRepository userRepository, ITransactionRepository transactionRepository)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateUserAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
        }

        public async Task<UnitPrice> SaveUnitPriceAsync(UnitPrice price)
        {
            return await _userRepository.SaveUnitPriceAsync(price);
        }

        public async Task<UnitPrice> UpdateUnitPriceAsync(UnitPrice price)
        {
            return await _userRepository.UpdateUnitPriceAsync(price);
        }

        public async Task DeleteUnitPriceAsync(int id)
        {
            await _userRepository.DeleteUnitPriceAsync(id);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByEmailAsync(string email)
        {
            return await _userRepository.GetTransactionsByEmailAsync(email);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(string email, DateTime from, DateTime to)
        {
            return await _userRepository.GetTransactionsByDateAsync(email, from, to);
        }

        public async Task<UnitPrice> GetUnitPriceAsync(int id)
        {
            return await _userRepository.GetUnitPriceAsync(id);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<bool> ShareUnitsAsync(string originAccountId, string destinationAccountId, decimal amount)
        {
            return await _userRepository.ShareUnitsAsync(originAccountId, destinationAccountId, amount);
        }

        public async Task<decimal> CalculateTotalCostAsync(string email)
        {
            return await _userRepository.CalculateTotalCostAsync(email);
        }

        public async Task<IEnumerable<UsageLimitDTO>> GetUsageLimitsByMonthAsync(string email, DateTime startDate, DateTime endDate)
        {
            return await _userRepository.GetUsageLimitsByMonthAsync(email, startDate, endDate);
        }

        public async Task UpdateBalanceAndLogTransactionAsync(string email, decimal amount, string accountId, string transactionType)
        {
            await _userRepository.UpdateCashWalletAsync(email, amount, accountId, transactionType);

            var transaction = new Transaction
            {
                Email = email,
                Amount = amount,
                TransactionDate = DateTime.Now,
                TransactionType = transactionType
            };

            await _userRepository.LogTransactionAsync(transaction);
        }

        public async Task<(decimal cashWalletBalance, decimal userWalletBalance)> DebitCashWalletAndCreditUserAsync(string email, decimal amount, string accountId)
        {
            return await _userRepository.DebitCashWalletAndCreditUserAsync(email, amount, accountId);
        }

        public async Task SaveDailyLimitsAsync(DailyLimit dailyLimit)
        {
            await _userRepository.SaveDailyLimitsAsync(dailyLimit);
        }

        public async Task<int> GetTotalCostCountAsync(string email)
        {
            return await _userRepository.GetTotalCostCountAsync(email);
        }
    }
}
