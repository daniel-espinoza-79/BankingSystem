using Domain.Entities.Abstracts;

namespace Domain.Entities.Concretes;

public class BankAccount: Registered<Guid>
{
    public int Balance { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public AccountType AccountType { get; set; }
    public string? Description { get; set; }
}