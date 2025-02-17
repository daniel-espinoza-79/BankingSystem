using Domain.Entities.Abstracts;

namespace Domain.Entities.Concretes;

public class Agent: Registered<Guid>
{
    public string? Descriptionn { get; set; }
    public TransactionMethod TransactionMethod { get; set; }
}