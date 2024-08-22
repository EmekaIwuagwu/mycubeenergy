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

        public async Task<User> GetUserByEmailOrIdAsync(string email, int userId)
        {
            if (!string.IsNullOrEmpty(email))
            {
                return await _userRepository.GetUserByEmailAsync(email);
            }

            return await _userRepository.GetUserByIdAsync(userId);
        }

        public async Task CalculateAndUpdateBillsUsageAsync(string email, TimeSpan usageDuration)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user != null)
            {
                // Calculate the total bills usage
                decimal totalUsage = (decimal)usageDuration.TotalHours * BillsUsageRatePerHour;

                // Update bills_use and unit_balance
                user.BillsUse += totalUsage;
                user.UnitBalance -= totalUsage;

                await _userRepository.UpdateUserAsync(user);
            }
        }

        public async Task UpdateBalanceAndLogTransactionAsync(string email, decimal amount)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user != null)
            {
                user.UnitBalance += amount;

                var transaction = new Transaction
                {
                    Email = email,
                    Amount = amount,
                    TransactionDate = DateTime.Now,
                    Description = "Payment received and balance updated"
                };

                await _userRepository.UpdateUserAsync(user);
                await _transactionRepository.AddTransactionAsync(transaction);
            }
        }

        /*public async Task<TotalCostDTO> CalculateTotalCostAsync(string email)
        {
            var totalCost = await _userRepository.CalculateTotalCostAsync(email);
            var count = await _userRepository.GetTotalCostCountAsync(email);

            return new TotalCostDTO
            {
                Email = email,
                Count = count,
                TotalCost = totalCost
            };
        }
        */

        public async Task<decimal> CalculateTotalCostAsync(string email)
        {
            return await _userRepository.CalculateTotalCostAsync(email);
        }

        public async Task<IEnumerable<UsageLimitDTO>> GetUsageLimitsByMonthAsync(string email, DateTime startDate, DateTime endDate)
        {
            return await _userRepository.GetUsageLimitsByMonthAsync(email, startDate, endDate);
        }

        public async Task<Result> ShareUnitsAsync(string originAccountId, string destinationAccountId, decimal amount)
        {
            var result = new Result();
            var success = await _userRepository.ShareUnitsAsync(originAccountId, destinationAccountId, amount);

            if (success)
            {
                result.Success = true;
            }
            else
            {
                result.Success = false;
                result.ErrorMessage = "Failed to share units. Please check account details or balance.";
            }

            return result;
        }
    }
}
