using System.Collections.Generic;

namespace Aggregates
{
	public abstract class AggregateRoot<TId> where TId : EntityId
	{
		public abstract TId Id { get; protected set; }

		public IEnumerable<DomainEvent> CurrentDomainEvents { get { return _currentDomainEvents; } }

		protected IList<DomainEvent> _currentDomainEvents = new List<DomainEvent>();
	}
}
