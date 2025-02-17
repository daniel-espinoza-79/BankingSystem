namespace Business.Dtos
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string AccountHolder { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}