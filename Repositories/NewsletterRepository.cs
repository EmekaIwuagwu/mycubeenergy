using CubeEnergy.Data;
using CubeEnergy.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CubeEnergy.Repositories
{
    public class NewsletterRepository : INewsletterRepository
    {
        private readonly ApplicationDbContext _context;

        public NewsletterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveSubscriberEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.");

            var existingSubscription = await _context.NewsletterSubscriptions
                .FirstOrDefaultAsync(n => n.Email.ToLower() == email.ToLower());

            if (existingSubscription != null)
                throw new InvalidOperationException("Email is already subscribed.");

            var subscription = new NewsletterSubscription
            {
                Email = email,
                SubscribedAt = DateTime.UtcNow
            };

            _context.NewsletterSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();
        }
    }
}
