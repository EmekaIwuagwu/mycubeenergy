using CubeEnergy.Models;
using CubeEnergy.Repositories;
using System;
using System.Threading.Tasks;

namespace CubeEnergy.Repositories
{
    public class InverterService : IInverterService
    {
        private readonly IUserRepository _userRepository;
        private const decimal DailyCostPerHour = 10.50m;

        public InverterService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<decimal> GetBalanceAsync(string accountId)
        {
            var user = await _userRepository.GetUserByAccountIdAsync(accountId);
            if (user == null)
                throw new Exception("User not found.");

            return user.UnitBalance;
        }

        public async Task SaveDailyLimitAsync(string email, string accountId, decimal hoursSpent)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                throw new Exception("User not found.");

            var dailyLimit = DailyCostPerHour * hoursSpent;
            user.UnitBalance -= dailyLimit;

            if (user.UnitBalance < 0)
                user.UnitBalance = 0; // Ensure balance does not go negative

            // Save updated user balance
            await _userRepository.UpdateUserAsync(user);

            // Save daily limit to database
            var dailyLimitRecord = new DailyLimit
            {
                AccountId = accountId,
                Email = email,
                DailyTotalCost = dailyLimit,
                Date = DateTime.UtcNow
            };

            await _userRepository.SaveDailyLimitAsync(dailyLimitRecord); // This should now work
        }
    }
}
