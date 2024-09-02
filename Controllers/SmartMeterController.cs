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
        public async Task<IActionResult> SaveDailyLimit([FromQuery] string email, [FromQuery] string accountId, [FromQuery] decimal hoursSpent)
        {
            try
            {
                await _inverterService.SaveDailyLimitAsync(email, accountId, hoursSpent);
                return Ok(new { Message = "Daily limit saved successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
