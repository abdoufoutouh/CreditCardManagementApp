using System.ComponentModel.DataAnnotations;

namespace CreditCardManagementApp.DTOS
{
    public class UpdateCreditCardDTO
    {
        [Required(ErrorMessage = "Credit limit is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Credit limit must be greater than 0")]
        public decimal CreditLimit { get; set; }

        [Required(ErrorMessage = "Current balance is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Current balance must be greater than or equal to 0")]
        public decimal CurrentBalance { get; set; }
    }
}
