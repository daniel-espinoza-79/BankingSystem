using Domain.Entities.Interfaces;

namespace Domain.Entities.Abstracts
{
    public abstract class Registered<TKey> : IEntity<TKey>
    {
        public TKey Id { get; set; }
        public DateTime RegisteredAt { get; set; }
        public bool IsActive { get; set; }
    }
}