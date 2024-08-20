using CubeEnergy.Data;
using CubeEnergy.Models;
using Microsoft.EntityFrameworkCore;

namespace CubeEnergy.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UnitPrice> SaveUnitPriceAsync(UnitPrice price)
        {
            _context.UnitPrices.Add(price);
            await _context.SaveChangesAsync();
            return price;
        }

        public async Task<UnitPrice> UpdateUnitPriceAsync(UnitPrice price)
        {
            _context.UnitPrices.Update(price);
            await _context.SaveChangesAsync();
            return price;
        }

        public async Task DeleteUnitPriceAsync(int id)
        {
            var price = await _context.UnitPrices.FindAsync(id);
            _context.UnitPrices.Remove(price);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByEmailAsync(string email)
        {
            return await _context.Transactions.Where(t => t.Email == email).ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(string email, DateTime from, DateTime to)
        {
            return await _context.Transactions.Where(t => t.Email == email && t.TransactionDate >= from && t.TransactionDate <= to).ToListAsync();
        }

        public async Task<UnitPrice> GetUnitPriceAsync(int id)
        {
            return await _context.UnitPrices.FindAsync(id);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

    }
}
