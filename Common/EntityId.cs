namespace Aggregates
{
	public abstract class EntityId
	{
		public EntityId(string id)
		{
			Id = id;
		}

		public string Id { get; private set; }

		public override string ToString()
		{
			return Id;
		}
	}
}
