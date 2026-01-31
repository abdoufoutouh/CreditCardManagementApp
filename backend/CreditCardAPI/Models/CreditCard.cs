using System;
using System.ComponentModel.DataAnnotations;

namespace CreditCardManagementApp.Models
{
    public class CreditCard
    {
        public int Id { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        public decimal CreditLimit { get; set; } = 0m;      
        [Required]
        public decimal CurrentBalance { get; set; } = 0m;

        public bool IsActive { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }   // relation vers User
    }
}
