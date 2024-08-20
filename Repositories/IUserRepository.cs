using CubeEnergy.Models;

namespace CubeEnergy.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int id);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<UnitPrice> SaveUnitPriceAsync(UnitPrice price);
        Task<UnitPrice> UpdateUnitPriceAsync(UnitPrice price);
        
        Task DeleteUnitPriceAsync(int id);
        Task<IEnumerable<Transaction>> GetTransactionsByEmailAsync(string email);
        Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(string email, DateTime from, DateTime to);
        Task<UnitPrice> GetUnitPriceAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
    }
}
