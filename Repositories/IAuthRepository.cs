using CubeEnergy.DTOs;
using CubeEnergy.Models;

namespace CubeEnergy.Repositories
{
    public interface IAuthRepository
    {
        Task SaveRefreshTokenAsync(string username, string refreshToken);
        Task<bool> ValidateRefreshTokenAsync(string username, string refreshToken);
        Task<string> GetUsernameByRefreshTokenAsync(string refreshToken);
        Task<User> RegisterUserAsync(User user);
        Task<User> GetUserByUsernameAsync(string username);
        Task<OTP> GenerateOTPAsync(string email, string otp);
        Task<OTP> ValidateOTPAsync(string email, string otp);
        Task UpdatePasswordAsync(User user, string newPassword);
    }
}
