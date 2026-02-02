using System.ComponentModel.DataAnnotations;

namespace CreditCardManagementApp.DTOS
{
    public class DeleteCreditCardDTO
    {
        [Required(ErrorMessage = "Credit card ID is required")]
        public int Id { get; set; }
    }
}
