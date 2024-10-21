using CubeEnergy.Data;
using CubeEnergy.Models;
using Microsoft.EntityFrameworkCore;

namespace CubeEnergy.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            if (user.RefreshToken == null)
            {
                user.RefreshToken = ""; // or some default value, if appropriate
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<OTP> GenerateOTPAsync(string email, string otp)
        {
            var newOtp = new OTP { Email = email, Code = otp };
            _context.OTPs.Add(newOtp);
            await _context.SaveChangesAsync();
            return newOtp;
        }

        public async Task<OTP> ValidateOTPAsync(string email, string otp)
        {
            return await _context.OTPs.FirstOrDefaultAsync(o => o.Email == email && o.Code == otp);
        }

        public async Task UpdatePasswordAsync(User user, string newPassword)
        {
            user.Password = newPassword;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task SaveRefreshTokenAsync(string username, string refreshToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ValidateRefreshTokenAsync(string username, string refreshToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            return user != null && user.RefreshToken == refreshToken;
        }

        public async Task<string> GetUsernameByRefreshTokenAsync(string refreshToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
            return user?.Username;
        }
    }
}
