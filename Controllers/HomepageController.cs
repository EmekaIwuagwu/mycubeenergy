// File: Controllers/HomepageController.cs

using CubeEnergy.DTOs;
using CubeEnergy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CubeEnergy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomepageController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly INewsletterService _newsletterService;
        private readonly ILogger<HomepageController> _logger;

        public HomepageController(EmailService emailService, INewsletterService newsletterService, ILogger<HomepageController> logger)
        {
            _emailService = emailService;
            _newsletterService = newsletterService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint to send Contact Us emails.
        /// </summary>
        /// <param name="contactUsDto">Contact form data.</param>
        /// <returns>Result message.</returns>
        [HttpPost("contact")]
        public async Task<IActionResult> ContactUs([FromBody] ContactUsDTO contactUsDto)
        {
            if (contactUsDto == null ||
                string.IsNullOrWhiteSpace(contactUsDto.Email) ||
                string.IsNullOrWhiteSpace(contactUsDto.Subject) ||
                string.IsNullOrWhiteSpace(contactUsDto.Message))
            {
                _logger.LogWarning("Invalid contact form data received.");
                return BadRequest(new { Message = "Invalid contact form data." });
            }

            try
            {
                // Capture current time and timezone
                var currentTime = DateTime.Now;
                var timezone = TimeZoneInfo.Local.DisplayName;

                // Format email body as HTML with proper line breaks and additional information
                var emailBody = $@"
                    <p><strong>From:</strong> {contactUsDto.Email}</p>
                    <p><strong>Message:</strong> {contactUsDto.Message}</p>
                    <p><strong>Time:</strong> {currentTime.ToString("hh:mm tt")}</p>
                    <p><strong>Timezone:</strong> {timezone}</p>
                ";

                // Send email to devops4@qstix.com.ng
                await _emailService.SendEmailAsync("devops4@qstix.com.ng", contactUsDto.Subject, emailBody);

                _logger.LogInformation("Contact email sent successfully from {Email}.", contactUsDto.Email);
                return Ok(new { Message = "Contact email sent successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending contact email from {Email}.", contactUsDto.Email);
                return StatusCode(500, new { Message = "An error occurred while sending the email." });
            }
        }

        /// <summary>
        /// Endpoint to subscribe to newsletters.
        /// </summary>
        /// <param name="email">Subscriber's email.</param>
        /// <returns>Result message.</returns>
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Empty email subscription attempt.");
                return BadRequest(new { Message = "Email cannot be empty." });
            }

            try
            {
                await _newsletterService.SubscribeAsync(email);
                _logger.LogInformation("New newsletter subscription: {Email}.", email);
                return Ok(new { Message = "Subscription successful." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Subscription failed for {Email}: {ErrorMessage}.", email, ex.Message);
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while subscribing {Email}.", email);
                return StatusCode(500, new { Message = "An error occurred while subscribing." });
            }
        }
    }
}
