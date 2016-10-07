using Journalist;
using Journalist.Options;
using System.Collections.Generic;

namespace Aggregates
{
	public sealed class Project : AggregateRoot<ProjectId>
	{
		public override ProjectId Id { get; protected set; }

		public string Name { get; private set; }

		public IEnumerable<ProjectType> ProjectTypes { get; private set; }

		public Option<string> Description { get; private set; }

		public void CreateNew(ProjectId projectId, string name)
		{
			Require.NotNull(projectId, nameof(projectId));
			AssertNotCreated();

			Id = projectId;
			ChangeName(name);
		}

		public void ChangeName(string name)
		{
			Require.NotEmpty(name, nameof(name));
			const int MaximumProjectNameLength = 40;
			Require.False(name.Length > MaximumProjectNameLength, nameof(name), "Name is too long");
			AssertCreated();

			Name = name;
		}

		public void ChangeProjectTypes(IEnumerable<ProjectType> projectTypes)
		{
			Require.NotEmpty(projectTypes, nameof(projectTypes));

			ProjectTypes = projectTypes;
		}

		private void AssertNotCreated()
		{
			Require.True(Id == null, Id.ToString(), "Project already created");
		}

		private void AssertCreated()
		{
			Require.NotNull(Id, nameof(Id));
		}
	}
}
