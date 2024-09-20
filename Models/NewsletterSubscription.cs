using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CubeEnergy.Models
{
    public class NewsletterSubscription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
    }
}
