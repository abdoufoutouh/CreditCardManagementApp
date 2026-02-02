namespace CreditCardManagementApp.DTOS
{
    public class CreditCardResponseDTO
    {
        public int Id { get; set; }
        public string CardNumberPartial { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal CurrentBalance { get; set; }
        public bool IsActive { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
