﻿using CubeEnergy.DTOs;
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

        public async Task UpdateBalanceAndLogTransactionAsync(
            string email,
            decimal amount,
            string accountId,
            string transactionType,
            string payerName,
            string packageType,
            int days,
            string paymentMethod)
        {
            // Fetch user to ensure the user exists
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            // Define the new balance based on transaction type
            decimal newBalance;

            if (transactionType == "Credit")
            {
                newBalance = user.UnitBalance + amount;  // Increase user's balance
            }
            else if (transactionType == "Debit")
            {
                if (user.UnitBalance < amount) // Check for sufficient balance
                {
                    throw new InvalidOperationException("Insufficient balance to complete the debit transaction.");
                }
                newBalance = user.UnitBalance - amount;  // Decrease user's balance
            }
            else
            {
                throw new ArgumentException("Invalid transaction type. Must be 'Credit' or 'Debit'.");
            }

            // Prepare the transaction object
            var transaction = new Transaction
            {
                Email = email,
                Amount = amount,
                TransactionType = transactionType,
                AccountId = accountId,
                Description = transactionType == "Credit"
                    ? $"Credited {amount:C} to {email}."
                    : $"Debited {amount:C} from {email}.",
                CreatedAt = DateTime.UtcNow
            };

            // Update the cash wallet and log the transaction
            await _userRepository.UpdateCashWalletAsync(email, amount, accountId, transactionType, payerName, packageType, days, paymentMethod);

            // Log the transaction
            await _transactionRepository.LogTransactionAsync(transaction);

            // Update the user's unit balance
            user.UnitBalance = newBalance; // Update the user's balance
            await _userRepository.UpdateUserAsync(user); // Ensure to save the user changes
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

        public async Task UpdateCashWalletAsync(string email, decimal amount, string accountId, string transactionType, string payerName, string packageType, int days, string paymentMethod)
        {
            await _userRepository.UpdateCashWalletAsync(email, amount, accountId, transactionType,payerName,packageType,days,paymentMethod);
        }

        public async Task<(decimal CashWalletBalance, decimal UserWalletBalance)> DebitCashWalletAndCreditUserAsync(string email, decimal amount, string accountId)
        {
            return await _userRepository.DebitCashWalletAndCreditUserAsync(email, amount, accountId);
        }

        public async Task InsertCashWalletAndTransactionAsync(string email, decimal amount, string accountId, string transactionType) 
        {
            await _userRepository.InsertCashWalletAndTransactionAsync(email, amount, accountId, transactionType);
        }

        public Task<UserDetailDTO> GetUserDetailsByEmailAsync(string email)
        {
            return _userRepository.GetUserDetailsByEmailAsync(email);
        }

        public async Task<IEnumerable<MonthlyTotalDTO>> GetMonthlyTotalCostAsync(string email, int year)
        {
            return await _userRepository.GetMonthlyTotalCostAsync(email, year);
        }

        public async Task<string> GetEmailByAccountIdAsync(string accountId)
        {
            return await _userRepository.GetEmailByAccountIdAsync(accountId);
        }

        public async Task<CashWallet> GetCashWalletByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }
    }
}
