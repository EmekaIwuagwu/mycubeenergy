using CubeEnergy.DTOs;
using CubeEnergy.Models;
using CubeEnergy.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CubeEnergy.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;

        public UserService(IUserRepository userRepository, ITransactionRepository transactionRepository)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<User> GetUserByEmailOrIdAsync(string email, int userId)
        {
            return await _userRepository.GetUserByEmailOrIdAsync(email, userId);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            return await _userRepository.DeleteUserAsync(userId);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateUserAsync(user);
        }

        public async Task<UnitPrice> GetUnitPriceAsync(int id)
        {
            return await _userRepository.GetUnitPriceAsync(id);
        }

        public async Task<UnitPrice> GetUnitPriceAsync()
        {
            return await _userRepository.GetUnitPriceAsync(); // Add method if required
        }

        public async Task UpdateBalanceAndLogTransactionAsync(string email, decimal amount, string accountId, string transactionType)
        {
            await _userRepository.UpdateCashWalletAsync(email, amount, accountId, transactionType);

            var transaction = new Transaction
            {
                Email = email,
                Amount = amount,
                TransactionType = transactionType,
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepository.LogTransactionAsync(transaction);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByEmailAsync(string email)
        {
            return await _transactionRepository.GetTransactionsByEmailAsync(email);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(string email, DateTime from, DateTime to)
        {
            return await _transactionRepository.GetTransactionsByDateAsync(email, from, to);
        }

        public async Task<Result> ShareUnitsAsync(string originAccountId, string destinationAccountId, decimal amount)
        {
            var result = await _userRepository.ShareUnitsAsync(originAccountId, destinationAccountId, amount);
            return result;
        }

        public async Task<decimal> CalculateTotalCostAsync(string email)
        {
            return await _userRepository.CalculateTotalCostAsync(email);
        }

        public async Task<IEnumerable<UsageLimitDTO>> GetUsageLimitsByMonthAsync(string email, DateTime startDate, DateTime endDate)
        {
            return await _userRepository.GetUsageLimitsByMonthAsync(email, startDate, endDate);
        }

        public async Task UpdateCashWalletAsync(string email, decimal amount, string accountId, string transactionType)
        {
            await _userRepository.UpdateCashWalletAsync(email, amount, accountId, transactionType);
        }

        public async Task<(decimal CashWalletBalance, decimal UserWalletBalance)> DebitCashWalletAndCreditUserAsync(string email, decimal amount, string accountId)
        {
            return await _userRepository.DebitCashWalletAndCreditUserAsync(email, amount, accountId);
        }
    }

}
