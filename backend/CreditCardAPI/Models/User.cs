using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CreditCardManagementApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        public List<CreditCard> CreditCards { get; set; } = new();
    }
}
