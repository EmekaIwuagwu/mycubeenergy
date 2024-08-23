﻿using CubeEnergy.DTOs;
using CubeEnergy.Models;
using CubeEnergy.Services;
using CubeEnergy.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CubeEnergy.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly JwtService _jwtService;

        public AdminController(IAdminService adminService, JwtService jwtService)
        {
            _adminService = adminService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(AdminDTO adminDto)
        {
            var result = await _adminService.RegisterAdminAsync(adminDto);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.ErrorMessage });
            }

            return Ok(new { Message = "Admin registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            var result = await _adminService.LoginAdminAsync(loginDto.Username, loginDto.Password);
            if (result == null)
            {
                return Unauthorized(new { Message = "Invalid credentials" });
            }

            var token = _jwtService.GenerateToken(loginDto.Username);
            var refreshToken = _jwtService.GenerateRefreshToken(loginDto.Username);

            return Ok(new
            {
                Token = token,
                RefreshToken = refreshToken,
                Admin = result
            });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPut("update-user/{accountId}")]
        public async Task<IActionResult> UpdateUser(string accountId, UpdateUserDTO updateUserDto)
        {
            var result = await _adminService.UpdateUserAsync(accountId, updateUserDto);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.ErrorMessage });
            }

            return Ok(new { Message = "User updated successfully" });
        }

        [HttpDelete("delete-user/{accountId}")]
        public async Task<IActionResult> DeleteUser(string accountId)
        {
            await _adminService.DeleteUserAsync(accountId);
            return Ok(new { Message = "User deleted successfully" });
        }

        [HttpDelete("deleteSuperAdmin/{id}")]
        public async Task<IActionResult> DeleteSuperAdmin(int id)
        {
            await _adminService.DeleteSuperAdminAsync(id);
            return Ok(new { Message = "Super admin deleted successfully" });
        }

        [HttpPut("updateSuperAdmin/{id}")]
        public async Task<IActionResult> UpdateSuperAdmin(int id, UpdateSuperAdminDTO updateSuperAdminDto)
        {
            var result = await _adminService.UpdateSuperAdminAsync(id, updateSuperAdminDto);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.ErrorMessage });
            }

            return Ok(new { Message = "Super admin updated successfully" });
        }

        [HttpGet("track-payments")]
        public async Task<IActionResult> TrackPayments(string emailOrAccountId)
        {
            var payments = await _adminService.GetPaymentsAsync(emailOrAccountId);
            return Ok(payments);
        }

        [HttpGet("payment/{accountId}")]
        public async Task<IActionResult> GetPaymentByAccountId(string accountId)
        {
            var payment = await _adminService.GetPaymentByAccountIdAsync(accountId);
            return Ok(payment);
        }

        [HttpPost("enterUnitPrice")]
        public async Task<IActionResult> EnterUnitPrice(UnitPriceDTO priceDto)
        {
            var result = await _adminService.SaveUnitPriceAsync(priceDto);
            return Ok(result);
        }

        [HttpPut("updateUnitPrice")]
        public async Task<IActionResult> UpdateUnitPrice(UnitPriceDTO priceDto)
        {
            var result = await _adminService.UpdateUnitPriceAsync(priceDto);
            return Ok(new { Message = "Price updated successfully", UnitPrice = result });
        }

        [HttpDelete("deleteUnitPrice/{id}")]
        public async Task<IActionResult> DeleteUnitPrice(int id)
        {
            await _adminService.DeleteUnitPriceAsync(id);
            return Ok(new { Message = "Price deleted successfully" });
        }

        [HttpPost("return-payment")]
        public async Task<IActionResult> ReturnPayment(ReturnPaymentDTO returnPaymentDto)
        {
            // Call the service to perform the return payment operation
            await _adminService.ReturnPaymentAsync(returnPaymentDto.DebitAccountId, returnPaymentDto.CreditAccountId, returnPaymentDto.Amount);

            // Return a success response
            return Ok(new { Message = "Payment returned successfully" });
        }
    }
}
