using CreditCardManagementApp.Data;
using CreditCardManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CreditCardManagementApp.Repositories
{
     public class CreditCardRepository : ICreditCardRepository
    {
        private readonly AppDbContext _context;

        public CreditCardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CreditCard> AddAsync(CreditCard creditCard)
        {
            _context.CreditCards.Add(creditCard);
            await _context.SaveChangesAsync();
            return creditCard;
        }

        public async Task<CreditCard?> FindByCardNumberAndUserIdAsync(string cardNumber, int userId)
        {
            return await _context.CreditCards
                .FirstOrDefaultAsync(c => c.CardNumber == cardNumber && c.UserId == userId);
        }

        public async Task<CreditCard?> FindByIdAsync(int id)
        {
            return await _context.CreditCards
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CreditCard> UpdateAsync(CreditCard creditCard)
        {
            _context.CreditCards.Update(creditCard);
            await _context.SaveChangesAsync();
            return creditCard;
        }

        public async Task<bool> DeleteAsync(CreditCard creditCard)
        {
            _context.CreditCards.Remove(creditCard);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<CreditCard>> GetByUserIdAsync(int userId)
        {
            return await _context.CreditCards
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<int> GetActiveCountByUserIdAsync(int userId)
        {
            return 0;
        }

        public async Task<int> GetTotalCountByUserIdAsync(int userId)
        {
            return await _context.CreditCards
                .CountAsync(c => c.UserId == userId);
        }
    }
}
