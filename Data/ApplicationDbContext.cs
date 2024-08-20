using CubeEnergy.Models;
using Microsoft.EntityFrameworkCore;

namespace CubeEnergy.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<UnitPrice> UnitPrices { get; set; }
        public DbSet<Kyc> Kycs { get; set; }
    }
}
