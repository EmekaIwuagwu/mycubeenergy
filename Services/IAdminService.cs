using CubeEnergy.DTOs;
using CubeEnergy.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CubeEnergy.Services
{
    public interface IAdminService
    {
        Task<Result> RegisterAdminAsync(AdminDTO adminDto);
        Task<AdminDTO> LoginAdminAsync(string username, string password);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<Result> UpdateUserAsync(string accountId, UpdateUserDTO updateUserDto);
        Task DeleteUserAsync(string accountId);
        Task DeleteSuperAdminAsync(int id);
        Task<Result> UpdateSuperAdminAsync(int id, UpdateSuperAdminDTO updateSuperAdminDto);
        Task<IEnumerable<Transaction>> GetPaymentsAsync(string emailOrAccountId);
        Task<Transaction> GetPaymentByAccountIdAsync(string accountId);
        Task<UnitPrice> SaveUnitPriceAsync(UnitPriceDTO priceDto);
        Task<UnitPrice> UpdateUnitPriceAsync(UnitPriceDTO priceDto);
        Task DeleteUnitPriceAsync(int id);
        Task<(int TotalUsers, decimal TotalTransactionAmount)> GetTransactionsAndTotalUsers();
        Task ReturnPaymentAsync(string debitAccountId, string creditAccountId, decimal amount);
    }
}
