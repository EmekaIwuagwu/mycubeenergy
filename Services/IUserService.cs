using CubeEnergy.DTOs;
using CubeEnergy.Models;

namespace CubeEnergy.Services
{
    public interface IUserService
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByEmailOrIdAsync(string email, int userId);
        Task InsertCashWalletAndTransactionAsync(string email, decimal amount, string accountId, string transactionType);
        Task<IEnumerable<MonthlyTotalDTO>> GetMonthlyTotalCostAsync(string email, int year);
        Task<bool> DeleteUserAsync(int userId);
        Task UpdateUserAsync(User user);
        Task<UnitPrice> GetUnitPriceAsync(int id);
        Task<UnitPrice> GetUnitPriceAsync(); // Method to get unit price without ID if needed
        Task UpdateBalanceAndLogTransactionAsync(string email, decimal amount, string accountId, string transactionType);
        Task<IEnumerable<Transaction>> GetTransactionsByEmailAsync(string email);
        Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(string email, DateTime from, DateTime to);
        Task<Result> ShareUnitsAsync(string originAccountId, string destinationAccountId, decimal amount);
        Task<decimal> CalculateTotalCostAsync(string email);
        Task<UserDetailDTO> GetUserDetailsByEmailAsync(string email);
        Task<IEnumerable<UsageLimitDTO>> GetUsageLimitsByMonthAsync(string email, DateTime startDate, DateTime endDate);
        Task UpdateCashWalletAsync(string email, decimal amount, string accountId, string transactionType);
        Task<(decimal CashWalletBalance, decimal UserWalletBalance)> DebitCashWalletAndCreditUserAsync(string email, decimal amount, string accountId);
    }

}
