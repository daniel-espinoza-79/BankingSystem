using Domain.Entities.Abstracts;

namespace Domain.Entities.Concretes
{
    public class User : Registered<Guid>
    {
        public string FullName { get; set; }
        public string DocumentId { get; set; }
        public List<BankAccount> BankAccounts { get; set; }
    }
}