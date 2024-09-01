using CubeEnergy.DTOs;
using CubeEnergy.Models;
using CubeEnergy.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CubeEnergy.Controllers
{
    [Route("api/User/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDTO registerDto)
        {
            var user = new User
            {
                Username = registerDto.Username,
                Password = registerDto.Password,
                Fullname = registerDto.FullName,
                Country = registerDto.Country,
                Address = registerDto.Address,
                Telephone = registerDto.Telephone,
                City = registerDto.City,
                State = registerDto.State,
                Zipcode = registerDto.Zipcode
            };

            var createdUser = await _authService.RegisterUserAsync(user);
            return Ok(new { Message = "User Created Successfully", User = createdUser });
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDTO loginDto)
        {
            var result = await _authService.LoginUserAsync(loginDto.Username, loginDto.Password);
            if (result == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(new
            {
                Message = "Login Successful",
                User = new
                {
                    Username = loginDto.Username
                },
                Token = result.Token,
                RefreshToken = result.RefreshToken
            });
        }

        [HttpPost("sendOTP")]
        public async Task<IActionResult> SendOTP([FromBody] string email)
        {
            var otp = await _authService.GenerateAndSendOTPAsync(email);
            return Ok(new { Message = "OTP Sent to your email.", OTP = otp });
        }

        [HttpPost("validateOTP")]
        public async Task<IActionResult> ValidateOTP([FromBody] ValidateOTPRequest request)
        {
            var isValid = await _authService.ValidateOTPAsync(request.Email, request.OTP);
            if (isValid)
            {
                return Ok("OTP Validated");
            }

            return BadRequest("Invalid or expired OTP");
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDto)
        {
            await _authService.UpdatePasswordAsync(changePasswordDto.Email, changePasswordDto.NewPassword);
            return Ok("Password Changed Successfully");
        }
    }
}
