using CubeEnergy.Data;
using CubeEnergy.DTOs;
using CubeEnergy.Models;
using CubeEnergy.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CubeEnergy.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AdminService(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<Result> RegisterAdminAsync(AdminDTO adminDto)
        {
            if (await _context.Admins.AnyAsync(a => a.AdminUsername == adminDto.AdminUsername))
            {
                return new Result { Success = false, ErrorMessage = "Admin already exists" };
            }

            var admin = new Admin
            {
                AdminUsername = adminDto.AdminUsername,
                AdminPassword = HashPassword(adminDto.AdminPassword),
                AdminType = adminDto.AdminType,
                CreatedAt = DateTime.UtcNow
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return new Result { Success = true };
        }

        public async Task<AdminDTO> LoginAdminAsync(string username, string password)
        {
            var admin = await _context.Admins
                .SingleOrDefaultAsync(a => a.AdminUsername == username);

            if (admin == null || !VerifyPassword(password, admin.AdminPassword))
            {
                return null;
            }

            return new AdminDTO
            {
                AdminUsername = admin.AdminUsername,
                AdminType = admin.AdminType
            };
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<Result> UpdateUserAsync(string accountId, UpdateUserDTO updateUserDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.AccountId == accountId);
            if (user == null)
            {
                return new Result { Success = false, ErrorMessage = "User not found" };
            }

            user.Fullname = updateUserDto.Fullname;
            user.Address = updateUserDto.Address;
            user.Telephone = updateUserDto.Telephone;
            user.City = updateUserDto.City;
            user.State = updateUserDto.State;
            user.Zipcode = updateUserDto.Zipcode;
            user.UnitBalance = updateUserDto.UnitBalance;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new Result { Success = true };
        }

        public async Task DeleteUserAsync(string accountId)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.AccountId == accountId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteSuperAdminAsync(int id)
        {
            var admin = await _context.Admins.SingleOrDefaultAsync(a => a.Id == id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Result> UpdateSuperAdminAsync(int id, UpdateSuperAdminDTO updateSuperAdminDto)
        {
            var admin = await _context.Admins.SingleOrDefaultAsync(a => a.Id == id);
            if (admin == null)
            {
                return new Result { Success = false, ErrorMessage = "Super Admin not found" };
            }

            admin.AdminUsername = updateSuperAdminDto.Username;
            admin.AdminPassword = HashPassword(updateSuperAdminDto.Password);
            admin.AdminType = updateSuperAdminDto.AdminType;

            _context.Admins.Update(admin);
            await _context.SaveChangesAsync();

            return new Result { Success = true };
        }

        public async Task<IEnumerable<Transaction>> GetPaymentsAsync(string emailOrAccountId)
        {
            var payments = await _context.Transactions
                .Where(t => t.Email == emailOrAccountId || t.AccountId == emailOrAccountId)
                .ToListAsync();

            return payments;
        }

        public async Task<Transaction> GetPaymentByAccountIdAsync(string accountId)
        {
            return await _context.Transactions
                .Where(t => t.AccountId == accountId)
                .SingleOrDefaultAsync();
        }

        public async Task<UnitPrice> SaveUnitPriceAsync(UnitPriceDTO priceDto)
        {
            var price = new UnitPrice { Price = priceDto.Price };
            _context.UnitPrices.Add(price);
            await _context.SaveChangesAsync();
            return price;
        }

        public async Task<UnitPrice> UpdateUnitPriceAsync(UnitPriceDTO priceDto)
        {
            var price = await _context.UnitPrices.FindAsync(priceDto.Id);
            if (price != null)
            {
                price.Price = priceDto.Price;
                _context.UnitPrices.Update(price);
                await _context.SaveChangesAsync();
            }
            return price;
        }

        public async Task DeleteUnitPriceAsync(int id)
        {
            var price = await _context.UnitPrices.FindAsync(id);
            if (price != null)
            {
                _context.UnitPrices.Remove(price);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ReturnPaymentAsync(string debitAccountId, string creditAccountId, decimal amount)
        {
            // Find the debit user by debitAccountId
            var debitUser = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == debitAccountId);
            if (debitUser == null)
            {
                throw new Exception("Debit user not found");
            }

            // Find the credit user by creditAccountId
            var creditUser = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == creditAccountId);
            if (creditUser == null)
            {
                throw new Exception("Credit user not found");
            }

            // Debit the user's balance
            debitUser.UnitBalance -= amount; // Assuming UnitBalance is the balance field

            // Credit the other user's balance
            creditUser.UnitBalance += amount; // Assuming UnitBalance is the balance field

            // Create debit transaction entry
            var debitTransaction = new Transaction
            {
                AccountId = debitAccountId,
                Amount = -amount, // Negative amount for debit
                TransactionType = "Debit",
                TransactionDate = DateTime.UtcNow,
                Description = "Refund processed"
            };

            // Create credit transaction entry
            var creditTransaction = new Transaction
            {
                AccountId = creditAccountId,
                Amount = amount, // Positive amount for credit
                TransactionType = "Credit",
                TransactionDate = DateTime.UtcNow,
                Description = "Refund received"
            };

            // Add transactions to the Transactions table
            _context.Transactions.Add(debitTransaction);
            _context.Transactions.Add(creditTransaction);

            // Save changes to the database
            await _context.SaveChangesAsync();
        }


        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            var hashedInput = HashPassword(inputPassword);
            return hashedInput == storedHash;
        }

        public async Task<(int TotalUsers, decimal TotalTransactionAmount)> GetTransactionsAndTotalUsers()
        {
            int totalUsers = await _context.Users.CountAsync();
            decimal totalTransactionAmount = await _context.Transactions.SumAsync(t => t.Amount);

            return (totalUsers, totalTransactionAmount);
        }
    }
}
