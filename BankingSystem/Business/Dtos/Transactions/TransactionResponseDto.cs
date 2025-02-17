using Domain.Entities.Concretes;
using TransactionStatus = System.Transactions.TransactionStatus;

namespace Business.Dtos.Transactions;

public class TransactionResponseDto
{
    public Guid Id { get; set; }
    public TransactionType TransactionType { get; set; }
    public TransactionStatus Status { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? Description { get; set; }
    
    public Guid? AccountOriginId { get; set; }
    public Guid? AccountDestinationId { get; set; }
    
    public Guid AgentId { get; set; }
}
