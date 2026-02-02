using CreditCardManagementApp.DTOS;
using CreditCardManagementApp.Models;
using CreditCardManagementApp.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace CreditCardManagementApp.Services
{
    public class CreditCardService : ICreditCardService
    {
        private readonly ICreditCardRepository _repository;

        private static readonly HashSet<string> BlacklistedCardNumbers = new()
        {
            "1111111111111111",
            "2222222222222222",
            "3333333333333333",
            "4444444444444444",
            "5555555555555555",
            "6666666666666666",
            "7777777777777777",
            "8888888888888888",
            "9999999999999999",
            "0000000000000000",
            "1234567890123456",
            "0123456789012345"
        };

        private static readonly Dictionary<string, string[]> CardTypePrefixes = new()
        {
            { "Visa", new[] { "4" } },
            { "Mastercard", new[] { "2221", "2222", "2223", "2224", "2225", "2226", "2227", "2228", "2229", "270", "271", "223", "224", "225", "226", "227", "228", "229", "51", "52", "53", "54", "55", "23", "24", "25", "26" } },
            { "Amex", new[] { "34", "37" } }
        };

        public CreditCardService(ICreditCardRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreditCardResponseDTO?> CreateCreditCardAsync(CreateCreditCardDTO dto, int userId)
        {
            // Vérifier l'unicité GLOBALE du numéro de carte (pas seulement par utilisateur)
            var existingCard = await _repository.FindByCardNumberAsync(dto.CardNumber);
            if (existingCard != null)
            {
                return null; // Numéro de carte déjà utilisé globalement
            }

            if (!ValidateCardNumber(dto.CardNumber))
            {
                return null;
            }

            var now = DateTime.UtcNow;
            var maxExpirationDate = now.AddYears(10);
            if (dto.ExpirationDate <= now || dto.ExpirationDate > maxExpirationDate)
            {
                return null;
            }

            if (dto.CreditLimit <= 0)
            {
                return null;
            }

            if (dto.CurrentBalance < 0 || dto.CurrentBalance > dto.CreditLimit)
            {
                return null;
            }

            var activeCardCount = await _repository.GetActiveCountByUserIdAsync(userId);
            var totalCardCount = await _repository.GetTotalCountByUserIdAsync(userId);

            if (activeCardCount >= 5)
            {
                return null;
            }

            if (totalCardCount >= 10)
            {
                return null;
            }

            if (!ValidateCardType(dto.Type, dto.CardNumber))
            {
                return null;
            }

            if (BlacklistedCardNumbers.Contains(dto.CardNumber))
            {
                return null;
            }

            var creditCard = new CreditCard
            {
                CardNumber = dto.CardNumber,
                ExpirationDate = dto.ExpirationDate,
                CreditLimit = dto.CreditLimit,
                CurrentBalance = dto.CurrentBalance,
                IsActive = false,
                Type = dto.Type,
                UserId = userId
            };

            var createdCard = await _repository.AddAsync(creditCard);

            return new CreditCardResponseDTO
            {
                Id = createdCard.Id,
                CardNumberPartial = GetLastFourDigits(createdCard.CardNumber),
                ExpirationDate = createdCard.ExpirationDate,
                CreditLimit = createdCard.CreditLimit,
                CurrentBalance = createdCard.CurrentBalance,
                IsActive = createdCard.IsActive,
                Type = createdCard.Type,
                Message = "Credit card created successfully"
            };
        }

        public async Task<CreditCardResponseDTO?> UpdateCreditCardAsync(int id, UpdateCreditCardDTO dto, int userId)
        {
            // Find existing credit card by ID and ensure it belongs to the user
            var existingCard = await _repository.FindByIdAsync(id);
            if (existingCard == null || existingCard.UserId != userId)
            {
                return null;
            }

            // Validate credit limit > 0
            if (dto.CreditLimit <= 0)
            {
                return null;
            }

            // Validate current balance ≥ 0 and ≤ credit limit
            if (dto.CurrentBalance < 0 || dto.CurrentBalance > dto.CreditLimit)
            {
                return null;
            }

            // Update only CreditLimit and CurrentBalance
            existingCard.CreditLimit = dto.CreditLimit;
            existingCard.CurrentBalance = dto.CurrentBalance;

            // Update the record
            var updatedCard = await _repository.UpdateAsync(existingCard);

            return new CreditCardResponseDTO
            {
                Id = updatedCard.Id,
                CardNumberPartial = GetLastFourDigits(updatedCard.CardNumber),
                ExpirationDate = updatedCard.ExpirationDate,
                CreditLimit = updatedCard.CreditLimit,
                CurrentBalance = updatedCard.CurrentBalance,
                IsActive = updatedCard.IsActive,
                Type = updatedCard.Type,
                Message = "Credit card updated successfully"
            };
        }

        public async Task<CreditCardResponseDTO?> DeleteCreditCardAsync(DeleteCreditCardDTO dto, int userId)
        {
            var existingCard = await _repository.FindByIdAsync(dto.Id);
            if (existingCard == null || existingCard.UserId != userId)
            {
                return null;
            }

            if (existingCard.CurrentBalance > 0)
            {
                return null;
            }

            var deleted = await _repository.DeleteAsync(existingCard);
            if (!deleted)
            {
                return null;
            }

            return new CreditCardResponseDTO
            {
                Id = existingCard.Id,
                CardNumberPartial = GetLastFourDigits(existingCard.CardNumber),
                ExpirationDate = existingCard.ExpirationDate,
                CreditLimit = existingCard.CreditLimit,
                CurrentBalance = existingCard.CurrentBalance,
                IsActive = existingCard.IsActive,
                Type = existingCard.Type,
                Message = "Credit card deleted successfully"
            };
        }

        public async Task<CreditCardResponseDTO?> GetCreditCardByIdAsync(int id, int userId)
        {
            var existingCard = await _repository.FindByIdAsync(id);
            if (existingCard == null || existingCard.UserId != userId)
            {
                return null;
            }

            return new CreditCardResponseDTO
            {
                Id = existingCard.Id,
                CardNumberPartial = GetLastFourDigits(existingCard.CardNumber),
                ExpirationDate = existingCard.ExpirationDate,
                CreditLimit = existingCard.CreditLimit,
                CurrentBalance = existingCard.CurrentBalance,
                IsActive = existingCard.IsActive,
                Type = existingCard.Type,
                Message = "Credit card retrieved successfully"
            };
        }

        public async Task<CreditCardListDTO> GetCreditCardsAsync(int userId, bool? isActive)
        {
            var cards = await _repository.GetByUserIdAsync(userId);

            if (isActive.HasValue)
            {
                cards = cards.Where(c => c.IsActive == isActive.Value).ToList();
            }

            var response = new CreditCardListDTO
            {
                Message = "Credit cards retrieved successfully"
            };

            foreach (var card in cards)
            {
                response.Cards.Add(new CreditCardResponseDTO
                {
                    Id = card.Id,
                    CardNumberPartial = GetLastFourDigits(card.CardNumber),
                    ExpirationDate = card.ExpirationDate,
                    CreditLimit = card.CreditLimit,
                    CurrentBalance = card.CurrentBalance,
                    IsActive = card.IsActive,
                    Type = card.Type,
                    Message = string.Empty
                });
            }

            return response;
        }

        public async Task<CreditCardListDTO> SearchCreditCardsAsync(int userId, string? cardType, string? cardNumber)
        {
            var cards = await _repository.GetByUserIdAsync(userId);

            // Filter by card type if provided
            if (!string.IsNullOrWhiteSpace(cardType))
            {
                cards = cards.Where(c => c.Type.Equals(cardType, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Filter by card number (last 4 digits) if provided
            if (!string.IsNullOrWhiteSpace(cardNumber))
            {
                var cleanedSearchNumber = cardNumber.Replace(" ", "").Replace("-", "");
                cards = cards.Where(c => GetLastFourDigits(c.CardNumber).EndsWith(cleanedSearchNumber)).ToList();
            }

            var response = new CreditCardListDTO
            {
                Message = "Search completed successfully"
            };

            foreach (var card in cards)
            {
                response.Cards.Add(new CreditCardResponseDTO
                {
                    Id = card.Id,
                    CardNumberPartial = GetLastFourDigits(card.CardNumber),
                    ExpirationDate = card.ExpirationDate,
                    CreditLimit = card.CreditLimit,
                    CurrentBalance = card.CurrentBalance,
                    IsActive = card.IsActive,
                    Type = card.Type,
                    Message = string.Empty
                });
            }

            return response;
        }

        private bool ValidateCardNumber(string cardNumber)
        {
            var cleanedNumber = cardNumber.Replace(" ", "").Replace("-", "");

            if (cleanedNumber.Length != 16 || !cleanedNumber.All(char.IsDigit))
            {
                return false;
            }

            return ValidateLuhnAlgorithm(cleanedNumber);
        }

        private bool ValidateLuhnAlgorithm(string cardNumber)
        {
            int sum = 0;
            bool alternate = false;

            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int digit = int.Parse(cardNumber[i].ToString());

                if (alternate)
                {
                    digit *= 2;
                    if (digit > 9)
                    {
                        digit -= 9;
                    }
                }

                sum += digit;
                alternate = !alternate;
            }

            return sum % 10 == 0;
        }

        private bool ValidateCardType(string cardType, string cardNumber)
        {
            var cleanedNumber = cardNumber.Replace(" ", "").Replace("-", "");

            if (!CardTypePrefixes.ContainsKey(cardType))
            {
                return false;
            }

            var prefixes = CardTypePrefixes[cardType];
            return prefixes.Any(prefix => cleanedNumber.StartsWith(prefix));
        }

        private string GetLastFourDigits(string cardNumber)
        {
            var cleanedNumber = cardNumber.Replace(" ", "").Replace("-", "");
            if (cleanedNumber.Length >= 4)
            {
                return cleanedNumber.Substring(cleanedNumber.Length - 4);
            }
            return cardNumber;
        }
    }
}
