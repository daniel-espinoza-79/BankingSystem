using Microsoft.AspNetCore.SignalR;

namespace Business.Dtos.Transactions;

public class TransactionRequestDto
{
    public Guid UserId { get; set; }
    public Guid AgentId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public Guid AccountOriginId { get; set; }
    public Guid AccountDestinationId { get; set; }
}