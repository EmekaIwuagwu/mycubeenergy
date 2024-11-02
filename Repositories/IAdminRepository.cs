using CubeEnergy.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CubeEnergy.Repositories
{
    public interface IAdminRepository
    {
        Task<Admin> RegisterAdminAsync(Admin admin);
        Task<Admin> GetAdminByUsernameAsync(string username);
        Task<IEnumerable<Admin>> GetAllAdminsAsync();
        Task<Admin> GetAdminByIdAsync(int id);
        Task UpdateAdminAsync(Admin admin);
        Task DeleteAdminAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByAccountIdAsync(string accountId);
        Task DeleteUserAsync(string accountId);
        Task<(int TotalUsers, decimal TotalTransactionAmount)> GetTransactionsAndTotalUsers();
        Task<IEnumerable<Transaction>> GetTransactionsByEmailOrAccountIdAsync(string emailOrAccountId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByReferenceNumberAsync(string referenceNumber);

    }
}
