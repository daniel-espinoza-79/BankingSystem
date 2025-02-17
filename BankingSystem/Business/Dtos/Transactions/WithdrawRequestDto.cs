namespace Business.Dtos.Transactions;

public class WithdrawRequestDto
{
    public Guid UserId { get; set; }
    public Guid AgentId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public Guid AccountId { get; set; }
}