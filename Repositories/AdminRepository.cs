using CubeEnergy.Data;
using CubeEnergy.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CubeEnergy.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Admin> RegisterAdminAsync(Admin admin)
        {
            _context.AdminTables.Add(admin);
            await _context.SaveChangesAsync();
            return admin;
        }

        public async Task<Admin> GetAdminByUsernameAsync(string username)
        {
            return await _context.AdminTables.FirstOrDefaultAsync(a => a.AdminUsername == username);
        }

        public async Task<IEnumerable<Admin>> GetAllAdminsAsync()
        {
            return await _context.AdminTables.ToListAsync();
        }

        public async Task<Admin> GetAdminByIdAsync(int id)
        {
            return await _context.AdminTables.FindAsync(id);
        }

        public async Task UpdateAdminAsync(Admin admin)
        {
            _context.AdminTables.Update(admin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAdminAsync(int id)
        {
            var admin = await GetAdminByIdAsync(id);
            if (admin != null)
            {
                _context.AdminTables.Remove(admin);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserByAccountIdAsync(string accountId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
        }

        public async Task DeleteUserAsync(string accountId)
        {
            var user = await GetUserByAccountIdAsync(accountId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByEmailOrAccountIdAsync(string emailOrAccountId)
        {
            return await _context.Transactions
                .Where(t => t.Email == emailOrAccountId || t.AccountId == emailOrAccountId)
                .ToListAsync();
        }
    }
}
