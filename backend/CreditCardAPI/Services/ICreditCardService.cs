using CreditCardManagementApp.DTOS;

namespace CreditCardManagementApp.Services
{
    public interface ICreditCardService
    {
        Task<CreditCardResponseDTO?> CreateCreditCardAsync(CreateCreditCardDTO dto, int userId);
        Task<CreditCardResponseDTO?> UpdateCreditCardAsync(UpdateCreditCardDTO dto, int userId);
    }
}
