using CubeEnergy.Models;
using System.Threading.Tasks;

namespace CubeEnergy.Repositories
{
    public interface IDailyLimitRepository
    {
        Task<DailyLimit> AddDailyLimitAsync(DailyLimit dailyLimit);
    }
}
