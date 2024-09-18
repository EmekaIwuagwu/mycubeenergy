using CubeEnergy.Repositories;
using CubeEnergy.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CubeEnergy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmartMeterController : ControllerBase
    {
        private readonly IInverterService _inverterService;

        public SmartMeterController(IInverterService inverterService)
        {
            _inverterService = inverterService;
        }

        [HttpGet("balance/{accountId}")]
        public async Task<IActionResult> GetBalance(string accountId)
        {
            try
            {
                var balance = await _inverterService.GetBalanceAsync(accountId);
                return Ok(new { AccountId = accountId, Balance = balance });
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost("saveDailyLimit")]
        public async Task<IActionResult> SaveDailyLimit(string accountId, decimal hoursSpent)
        {
            try
            {
                await _inverterService.SaveDailyLimitAsync(accountId, hoursSpent);
                return Ok(new { Message = "Daily limit saved successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("registerSmartMeter")]
        public async Task<IActionResult> RegisterSmartMeter(string macAddress, string accountId)
        {
            try
            {
                await _inverterService.RegisterSmartMeterAsync(macAddress, accountId);
                return Ok(new { Message = "Smart meter registered successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("getAccountId/{macAddress}")]
        public async Task<IActionResult> GetAccountIdByMacAddress(string macAddress)
        {
            try
            {
                var accountId = await _inverterService.GetAccountIdByMacAddressAsync(macAddress);
                return Ok(new { Message = "AccountId retrieved successfully", AccountId = accountId });
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
