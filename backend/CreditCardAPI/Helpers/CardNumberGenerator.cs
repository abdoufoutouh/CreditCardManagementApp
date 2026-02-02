using System;
using System.Linq;

namespace CreditCardManagementApp.Helpers
{
    public static class CardNumberGenerator
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Génère un numéro de carte valide selon le type
        /// </summary>
        public static string GenerateCardNumber(string cardType)
        {
            return cardType switch
            {
                "Visa" => GenerateVisaNumber(),
                "Mastercard" => GenerateMastercardNumber(),
                "Amex" => GenerateAmexNumber(),
                _ => GenerateVisaNumber() // Default to Visa
            };
        }

        private static string GenerateVisaNumber()
        {
            // Visa commence par 4
            string prefix = "4";
            return GenerateNumberWithLuhn(prefix, 16);
        }

        private static string GenerateMastercardNumber()
        {
            // Mastercard commence par 51-55 ou 2221-2720
            string[] prefixes = { "51", "52", "53", "54", "55" };
            string prefix = prefixes[_random.Next(prefixes.Length)];
            return GenerateNumberWithLuhn(prefix, 16);
        }

        private static string GenerateAmexNumber()
        {
            // Amex commence par 34 ou 37
            string[] prefixes = { "34", "37" };
            string prefix = prefixes[_random.Next(prefixes.Length)];
            return GenerateNumberWithLuhn(prefix, 16);
        }

        /// <summary>
        /// Génère un numéro avec un préfixe donné et calcule le chiffre de contrôle Luhn
        /// </summary>
        private static string GenerateNumberWithLuhn(string prefix, int totalLength)
        {
            // Générer les chiffres aléatoires (sauf le dernier qui sera le checksum)
            int digitsToGenerate = totalLength - prefix.Length - 1;
            string randomDigits = string.Join("", Enumerable.Range(0, digitsToGenerate)
                .Select(_ => _random.Next(0, 10)));

            string numberWithoutChecksum = prefix + randomDigits;

            // Calculer le chiffre de contrôle Luhn
            int checksum = CalculateLuhnChecksum(numberWithoutChecksum);

            return numberWithoutChecksum + checksum;
        }

        /// <summary>
        /// Calcule le chiffre de contrôle Luhn pour un numéro partiel
        /// </summary>
        private static int CalculateLuhnChecksum(string partialNumber)
        {
            int sum = 0;
            bool alternate = true; // Commence à true car on ajoute un chiffre à la fin

            // Parcourir de droite à gauche
            for (int i = partialNumber.Length - 1; i >= 0; i--)
            {
                int digit = int.Parse(partialNumber[i].ToString());

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

            // Le checksum est le chiffre qui rend la somme divisible par 10
            return (10 - (sum % 10)) % 10;
        }

        /// <summary>
        /// Valide qu'un numéro passe l'algorithme de Luhn
        /// </summary>
        public static bool ValidateLuhn(string cardNumber)
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
    }
}
