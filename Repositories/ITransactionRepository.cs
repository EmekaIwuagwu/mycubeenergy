using CubeEnergy.Models;
using System.Threading.Tasks;

namespace CubeEnergy.Repositories
{
    public interface ITransactionRepository
    {
        Task AddTransactionAsync(Transaction transaction);
    }
}
