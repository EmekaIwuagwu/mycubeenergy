using CubeEnergy.Data;
using CubeEnergy.Models;
using CubeEnergy.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CubeEnergy.Services
{
    public class InverterService : IInverterService
    {
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;
        private const decimal DailyCostPerHour = 10.50m;

        public InverterService(IUserRepository userRepository, ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<decimal> GetBalanceAsync(string accountId)
        {
            var user = await _userRepository.GetUserByAccountIdAsync(accountId);
            if (user == null)
                throw new Exception("User not found.");

            return user.UnitBalance;
        }

        public async Task SaveDailyLimitAsync(string accountId, decimal hoursSpent)
        {
            var email = await _userRepository.GetEmailByAccountIdAsync(accountId);
            var user = await _userRepository.GetUserByAccountIdAsync(accountId);
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
                Email = email,
                AccountId = accountId,
                DailyTotalCost = dailyLimit,
                Date = DateTime.UtcNow
            };

            await _userRepository.SaveDailyLimitsAsync(dailyLimitRecord); // This should now work
        }

        public async Task RegisterSmartMeterAsync(string macAddress, string accountId)
        {
            var smartMeter = new SmartMeter
            {
                MacAddress = macAddress,
                AccountId = accountId,
                CreatedAt = DateTime.UtcNow
            };

            _context.SmartMeters.Add(smartMeter);
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetAccountIdByMacAddressAsync(string macAddress)
        {
            var smartMeter = await _context.SmartMeters
             .FirstOrDefaultAsync(sm => sm.MacAddress == macAddress);

            if (smartMeter == null)
                throw new Exception("Smart meter not found.");

            return smartMeter.AccountId;
        }
    }
}
