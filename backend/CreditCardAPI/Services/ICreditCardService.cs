using CreditCardManagementApp.DTOS;

namespace CreditCardManagementApp.Services
{
    public interface ICreditCardService
    {
        Task<CreditCardResponseDTO?> CreateCreditCardAsync(CreateCreditCardDTO dto, int userId);
        Task<CreditCardResponseDTO?> UpdateCreditCardAsync(UpdateCreditCardDTO dto, int userId);
        Task<CreditCardResponseDTO?> DeleteCreditCardAsync(DeleteCreditCardDTO dto, int userId);
        Task<CreditCardResponseDTO?> GetCreditCardByIdAsync(int id, int userId);
        Task<CreditCardListDTO> GetCreditCardsAsync(int userId, bool? isActive);
    }
}
