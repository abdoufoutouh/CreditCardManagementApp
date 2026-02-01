using System.ComponentModel.DataAnnotations;

namespace CreditCardManagementApp.DTOS
{
    public class UpdateCreditCardDTO
    {
        [Required(ErrorMessage = "Credit card ID is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Card number is required")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Card number must be exactly 16 digits")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Card number must contain only digits")]
        public string CardNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Expiration date is required")]
        public DateTime ExpirationDate { get; set; }

        [Required(ErrorMessage = "Credit limit is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Credit limit must be greater than 0")]
        public decimal CreditLimit { get; set; }

        [Required(ErrorMessage = "Current balance is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Current balance must be greater than or equal to 0")]
        public decimal CurrentBalance { get; set; }

        [Required(ErrorMessage = "Card type is required")]
        [RegularExpression(@"^(Visa|Mastercard|Amex)$", ErrorMessage = "Card type must be Visa, Mastercard, or Amex")]
        public string Type { get; set; } = string.Empty;
    }
}
