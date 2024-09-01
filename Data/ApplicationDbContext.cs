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
        public DbSet<Admin> AdminTables { get; set; }
        public DbSet<Kyc> Kycs { get; set; }
        public DbSet<Limit> Limits { get; set; }
        public DbSet<DailyLimit> DailyLimits { get; set; }
        public DbSet<Admin> Admins { get; set; } // Added DbSet for Admins
        public DbSet<CashWallet> CashWallets { get; set; }
    }
}
