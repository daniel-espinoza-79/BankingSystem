using System.Transactions;
using Domain.Entities.Concretes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DataAccess.Context
{
    public class DataAccessContext(DbContextOptions<DataAccessContext> options) : DbContext(options)
    {
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<ATM> ATMs { get; set; }
        public DbSet<WebPlatform> WebPlatforms { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>().ToTable("Agents");

            modelBuilder.Entity<ATM>().ToTable("ATMs");
            modelBuilder.Entity<WebPlatform>().ToTable("WebPlatforms");
            
            modelBuilder.Entity<ATM>().HasData(
                new ATM
                {
                    Id = Guid.NewGuid(), Location = "Main Street 123", IsActive = true , TransactionMethod = TransactionMethod.Atm,
                    CurrentBalance = 5600
                },
                new ATM
                {
                    Id = Guid.NewGuid(), Location = "Central Park", IsActive = true , TransactionMethod = TransactionMethod.Atm,
                    CurrentBalance = 6700
                },
                new ATM
                {
                    Id = Guid.NewGuid(), Location = "Airport Terminal", IsActive = true , TransactionMethod = TransactionMethod.Atm,
                    CurrentBalance = 9000
                }
            );

            modelBuilder.Entity<WebPlatform>().HasData(
                new WebPlatform { Id = Guid.NewGuid(), Address = "https://bank.example.com", IsActive = true, Name = "WebPlatform",TransactionMethod = TransactionMethod.WebPlatform }
            );
        }
    }
}