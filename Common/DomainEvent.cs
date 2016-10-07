using System;

namespace Aggregates
{
	public abstract class DomainEvent
	{
		public DomainEvent(DateTimeOffset occuredOn)
		{
			OccuredOn = occuredOn;
		}

		public DateTimeOffset OccuredOn { get; private set; }
	}
}
