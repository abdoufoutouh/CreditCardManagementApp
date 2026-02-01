using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditCardAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixCreditCardMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // This migration is intentionally empty
            // We are fixing entity mappings without changing database schema
            // The database already has the correct structure:
            // - CurrentBalance column exists
            // - CardType column exists  
            // - IsActive column does not exist (marked as NotMapped)
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This migration is intentionally empty
            // No database changes were made in Up method
        }
    }
}
