using Microsoft.EntityFrameworkCore;
// Ajoute cette ligne pour trouver tes modèles
using CreditCardManagementApp.Models;

namespace CreditCardManagementApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CreditCard>()
                .Property(c => c.CreditLimit)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CreditCard>()
                .Property(c => c.CurrentBalance)
                .HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}
