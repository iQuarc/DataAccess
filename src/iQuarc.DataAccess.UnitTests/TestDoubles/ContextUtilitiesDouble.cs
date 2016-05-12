using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moq;

namespace iQuarc.DataAccess.UnitTests.TestDoubles
{
	class ContextUtilitiesDouble : IDbContextUtilities
	{
		private readonly Dictionary<int, IEnumerable<IEntityEntry>> changedAtCall = new Dictionary<int, IEnumerable<IEntityEntry>>();
		private int getChangedEntitiesCount = -1;

		public ContextUtilitiesDouble()
			: this(Enumerable.Empty<IEntityEntry>())
		{
		}

		public ContextUtilitiesDouble(IEnumerable<object> changedEntities, EntityEntryState state = EntityEntryState.Modified)
		{
			changedAtCall[0] = changedEntities.Select(e => new EntityEntryDouble(e, state));
		}

		public void AddEntitiesByCallNumber(int callCount, IEnumerable<object> entities, EntityEntryState state = EntityEntryState.Modified)
		{
			changedAtCall[callCount - 1] = entities.Select(e => new EntityEntryDouble(e, state));
		}

		public void AddEntriesByCallNumber(int callCount, IEnumerable<IEntityEntry> entries, EntityEntryState state = EntityEntryState.Modified)
		{
			changedAtCall[callCount - 1] = entries;
		}

		public IEnumerable<IEntityEntry> GetChangedEntities(DbContext context, Predicate<EntityState> statePredicate)
		{
			getChangedEntitiesCount++;

			IEnumerable<IEntityEntry> changedEntities;
			if (changedAtCall.TryGetValue(getChangedEntitiesCount, out changedEntities))
				return changedEntities;

			return changedAtCall[0];
		}

		public IEntityEntry GetEntry(object entity, DbContext context)
		{
			Mock<IEntityEntry> entryStub = new Mock<IEntityEntry>();
			entryStub.Setup(e => e.Entity).Returns(entity);
			return entryStub.Object;
		}

	    public IEntityEntry<T> GetEntry<T>(T entity, DbContext context) where T : class
	    {
            Mock<IEntityEntry<T>> entryStub = new Mock<IEntityEntry<T>>();
            entryStub.Setup(e => e.Entity).Returns(entity);
            return entryStub.Object;
        }

	    public IEnumerable<IEntityEntry> GetEntries(DbContext context)
	    {
	        yield break;
	    }
	}
}