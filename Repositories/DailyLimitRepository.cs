using CubeEnergy.Data;
using CubeEnergy.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CubeEnergy.Repositories
{
    public class DailyLimitRepository : IDailyLimitRepository
    {
        private readonly ApplicationDbContext _context;

        public DailyLimitRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DailyLimit> AddDailyLimitAsync(DailyLimit dailyLimit)
        {
            _context.DailyLimits.Add(dailyLimit);
            await _context.SaveChangesAsync();
            return dailyLimit;
        }
    }
}
