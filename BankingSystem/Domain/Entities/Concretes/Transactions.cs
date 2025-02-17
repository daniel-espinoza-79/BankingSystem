using Domain.Entities.Abstracts;

namespace Domain.Entities.Concretes
{
    public class Transactions : Registered<Guid>
    {
        public Guid AccountOriginId { get; set; }
        public Guid AccountDestinationId { get; set; }
        public Guid AgentId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
        public Agent Agent { get; set; }
    }
}