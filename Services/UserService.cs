using CubeEnergy.Models;
using CubeEnergy.Repositories;

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

        // Add GetUnitPriceAsync method
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
    }
}
