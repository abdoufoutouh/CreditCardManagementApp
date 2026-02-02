using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditCardManagementApp.Models
{
    public class CreditCard
    {
        public int Id { get; set; }

        [Required]
        [StringLength(16)]
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        public decimal CreditLimit { get; set; } = 0m;

        [Required]
        [Column("CurrentBalance")]
        public decimal CurrentBalance { get; set; } = 0m;

        [NotMapped]
        public bool IsActive { get; set; } = false;

        [Required]
        [StringLength(20)]
        [Column("CardType")]
        public string Type { get; set; } = string.Empty;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
