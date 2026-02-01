using CreditCardManagementApp.DTOS;
using CreditCardManagementApp.Models;
using CreditCardManagementApp.Repositories;

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
            var existingCard = await _repository.FindByCardNumberAndUserIdAsync(dto.CardNumber, userId);
            if (existingCard != null)
            {
                return null;
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
                CreditLimit = createdCard.CreditLimit,
                CurrentBalance = createdCard.CurrentBalance,
                IsActive = createdCard.IsActive,
                Message = "Credit card created successfully"
            };
        }

        public async Task<CreditCardResponseDTO?> UpdateCreditCardAsync(UpdateCreditCardDTO dto, int userId)
        {
            // Find existing credit card by ID and ensure it belongs to the user
            var existingCard = await _repository.FindByIdAsync(dto.Id);
            if (existingCard == null || existingCard.UserId != userId)
            {
                return null;
            }

            // Check if new card number is unique per user (except current record)
            if (dto.CardNumber != existingCard.CardNumber)
            {
                var duplicateCard = await _repository.FindByCardNumberAndUserIdAsync(dto.CardNumber, userId);
                if (duplicateCard != null)
                {
                    return null;
                }
            }

            // Validate card number format and Luhn algorithm
            if (!ValidateCardNumber(dto.CardNumber))
            {
                return null;
            }

            // Validate expiration date (future and ≤ 10 years)
            var now = DateTime.UtcNow;
            var maxExpirationDate = now.AddYears(10);
            if (dto.ExpirationDate <= now || dto.ExpirationDate > maxExpirationDate)
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

            // Validate card type and prefix match
            if (!ValidateCardType(dto.Type, dto.CardNumber))
            {
                return null;
            }

            // Check for blacklisted card numbers
            if (BlacklistedCardNumbers.Contains(dto.CardNumber))
            {
                return null;
            }

            // Map DTO to Entity
            existingCard.CardNumber = dto.CardNumber;
            existingCard.ExpirationDate = dto.ExpirationDate;
            existingCard.CreditLimit = dto.CreditLimit;
            existingCard.CurrentBalance = dto.CurrentBalance;
            existingCard.Type = dto.Type;
            // Note: IsActive is not mapped to database, so we don't update it here

            // Update the record
            var updatedCard = await _repository.UpdateAsync(existingCard);

            return new CreditCardResponseDTO
            {
                Id = updatedCard.Id,
                CardNumberPartial = GetLastFourDigits(updatedCard.CardNumber),
                CreditLimit = updatedCard.CreditLimit,
                CurrentBalance = updatedCard.CurrentBalance,
                IsActive = updatedCard.IsActive,
                Message = "Credit card updated successfully"
            };
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
