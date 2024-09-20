using CubeEnergy.Repositories;
using System;
using System.Threading.Tasks;

namespace CubeEnergy.Services
{
    public class NewsletterService : INewsletterService
    {
        private readonly INewsletterRepository _newsletterRepository;

        public NewsletterService(INewsletterRepository newsletterRepository)
        {
            _newsletterRepository = newsletterRepository;
        }

        public async Task SubscribeAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.");

            // Additional validation can be added here, such as email format verification.

            await _newsletterRepository.SaveSubscriberEmailAsync(email);
        }
    }
}
