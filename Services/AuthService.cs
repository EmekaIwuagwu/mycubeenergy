using CubeEnergy.Data;
using CubeEnergy.DTOs;
using CubeEnergy.Models;
using CubeEnergy.Repositories;
using CubeEnergy.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CubeEnergy.Services
{
    public class AuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtService _jwtService;
        private readonly BcryptService _bcryptService;
        private readonly EmailService _emailService;
        private readonly ApplicationDbContext _context;

        public AuthService(IAuthRepository authRepository, JwtService jwtService, BcryptService bcryptService, EmailService emailService, ApplicationDbContext context)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
            _bcryptService = bcryptService;
            _emailService = emailService;
            _context = context;
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            user.Password = _bcryptService.HashPassword(user.Password);
            return await _authRepository.RegisterUserAsync(user);
        }

        public async Task<LoginResponseDTO> LoginUserAsync(string username, string password)
        {
            var user = await _authRepository.GetUserByUsernameAsync(username);
            if (user == null || !_bcryptService.VerifyPassword(password, user.Password))
            {
                return null;
            }

            var token = _jwtService.GenerateToken(username);
            var refreshToken = _jwtService.GenerateRefreshToken(username);

            // Save the refresh token in the database
            await _authRepository.SaveRefreshTokenAsync(username, refreshToken);

            return new LoginResponseDTO
            {
                Token = token,
                RefreshToken = refreshToken
            };
        }

        public async Task<string> GenerateAndSendOTPAsync(string email)
        {
            var otp = GenerateOTP();
            var otpEntity = new OTP
            {
                Email = email,
                Code = otp,
                ExpiryTime = DateTime.Now.AddMinutes(5) // OTP valid for 5 minutes
            };

            _context.OTPs.Add(otpEntity);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(email, "Your OTP Code", $"Your OTP code is {otp}");

            return otp;
        }

        public string GenerateOTP()
        {
            var random = new Random();
            var otp = random.Next(100000, 999999); // Generates a 6-digit OTP
            return otp.ToString();
        }

        public async Task<bool> ValidateOTPAsync(string email, string otp)
        {
            var otpEntity = await _context.OTPs.FirstOrDefaultAsync(o => o.Email == email && o.Code == otp);

            if (otpEntity == null || otpEntity.ExpiryTime < DateTime.Now)
            {
                return false;
            }

            _context.OTPs.Remove(otpEntity); // Delete OTP after validation
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task UpdatePasswordAsync(string email, string newPassword)
        {
            var user = await _authRepository.GetUserByUsernameAsync(email);
            if (user != null)
            {
                await _authRepository.UpdatePasswordAsync(user, _bcryptService.HashPassword(newPassword));
            }
        }

        public async Task<string> RefreshTokenAsync(string refreshToken)
        {
            var username = await _authRepository.GetUsernameByRefreshTokenAsync(refreshToken);
            if (username != null)
            {
                var user = await _authRepository.GetUserByUsernameAsync(username);
                if (user != null && _jwtService.ValidateRefreshToken(refreshToken))
                {
                    var newToken = _jwtService.GenerateToken(user.Username);
                    var newRefreshToken = _jwtService.GenerateRefreshToken(user.Username);

                    // Save the new refresh token in the database
                    await _authRepository.SaveRefreshTokenAsync(user.Username, newRefreshToken);

                    return newToken;
                }
            }

            return null;
        }
    }
}
