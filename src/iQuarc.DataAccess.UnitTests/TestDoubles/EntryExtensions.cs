namespace iQuarc.DataAccess.UnitTests.TestDoubles
{
	static class EntryExtensions
	{
		public static IEntityEntry AsEntry(this object entity, EntityEntryState state = EntityEntryState.Modified)
		{
			return new EntityEntryDouble(entity, state);
		}
	}
}