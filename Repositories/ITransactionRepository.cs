using CubeEnergy.Models;
using System.Threading.Tasks;

namespace CubeEnergy.Repositories
{
    public interface ITransactionRepository
    {
        Task AddTransactionAsync(Transaction transaction);
        Task LogTransactionAsync(Transaction transaction); // Add this method
        Task<IEnumerable<Transaction>> GetTransactionsByEmailAsync(string email); // Add this method
        Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(string email, DateTime from, DateTime to); // Add this method
    }
}
