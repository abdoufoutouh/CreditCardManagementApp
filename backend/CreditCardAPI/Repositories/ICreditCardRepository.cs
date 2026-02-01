using CreditCardManagementApp.Models;

namespace CreditCardManagementApp.Repositories
{
    public interface ICreditCardRepository
    {
        Task<CreditCard> AddAsync(CreditCard creditCard);
        Task<CreditCard?> FindByCardNumberAndUserIdAsync(string cardNumber, int userId);
        Task<CreditCard?> FindByIdAsync(int id);
        Task<CreditCard> UpdateAsync(CreditCard creditCard);
        Task<List<CreditCard>> GetByUserIdAsync(int userId);
        Task<int> GetActiveCountByUserIdAsync(int userId);
        Task<int> GetTotalCountByUserIdAsync(int userId);
    }
}
