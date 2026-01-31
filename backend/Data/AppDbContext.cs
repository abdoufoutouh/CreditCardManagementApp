using Microsoft.EntityFrameworkCore;
using CreditCardAPI.Models;

namespace CreditCardAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
