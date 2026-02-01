using Microsoft.EntityFrameworkCore;
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

            modelBuilder.Entity<CreditCard>()
                .HasOne(c => c.User)
                .WithMany(u => u.CreditCards)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .HasMaxLength(500);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.FirstName)
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.LastName)
                .HasMaxLength(50);

            base.OnModelCreating(modelBuilder);
        }
    }
}
