namespace Domain.Entities.Concretes;

public class ATM: Agent
{
    public int CurrentBalance { get; set; }
    public string Location { get; set; }
}