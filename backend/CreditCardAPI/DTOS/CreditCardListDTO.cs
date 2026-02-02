using System.Collections.Generic;

namespace CreditCardManagementApp.DTOS
{
    public class CreditCardListDTO
    {
        public List<CreditCardResponseDTO> Cards { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}
