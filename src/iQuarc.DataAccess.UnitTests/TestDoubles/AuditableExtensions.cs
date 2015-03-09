namespace iQuarc.DataAccess.UnitTests.TestDoubles
{
    static class AuditableExtensions
    {
        public static IEntityEntry<IAuditable> AsAuditableEntry<T>(this T entity, EntityEntryState state = EntityEntryState.Unchanged)
            where T : IAuditable
        {
            return new EntityEntryDouble<IAuditable>(entity, state);
        }
    }
}