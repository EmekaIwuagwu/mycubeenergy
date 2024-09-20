// File: Controllers/HomepageController.cs
using CubeEnergy.DTOs;
using CubeEnergy.Services;
using Microsoft.AspNetCore.Mvc;
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

        public HomepageController(EmailService emailService, INewsletterService newsletterService)
        {
            _emailService = emailService;
            _newsletterService = newsletterService;
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
                return BadRequest(new { Message = "Invalid contact form data." });
            }

            try
            {
                // Send email to devops4@qstix.com.ng
                var emailBody = $"From: {contactUsDto.Email}\n\n{contactUsDto.Message}";
                await _emailService.SendEmailAsync("devops4@qstix.com.ng", contactUsDto.Subject, emailBody);

                return Ok(new { Message = "Contact email sent successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
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
                return BadRequest(new { Message = "Email cannot be empty." });
            }

            try
            {
                await _newsletterService.SubscribeAsync(email);
                return Ok(new { Message = "Subscription successful." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, new { Message = "An error occurred while subscribing." });
            }
        }
    }
}
