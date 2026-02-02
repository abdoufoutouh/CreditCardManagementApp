using CreditCardManagementApp.DTOS;

namespace CreditCardManagementApp.Services
{
    public interface ICreditCardService
    {
        Task<CreditCardResponseDTO?> CreateCreditCardAsync(CreateCreditCardDTO dto, int userId);
        Task<CreditCardResponseDTO?> UpdateCreditCardAsync(int id, UpdateCreditCardDTO dto, int userId);
        Task<CreditCardResponseDTO?> DeleteCreditCardAsync(DeleteCreditCardDTO dto, int userId);
        Task<CreditCardResponseDTO?> GetCreditCardByIdAsync(int id, int userId);
        Task<CreditCardListDTO> GetCreditCardsAsync(int userId, bool? isActive);
        Task<CreditCardListDTO> SearchCreditCardsAsync(int userId, string? cardType, string? cardNumber);
    }
}
