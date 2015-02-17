namespace iQuarc.DataAccess.UnitTests.TestDoubles
{
    static class AuditableExtensions
    {
        public static IEntityEntry<IAuditable> AsEntry<T>(this T entity, EntityEntryStates state = EntityEntryStates.Unchanged)
            where T : IAuditable
        {
            return new EntityEntryDouble<IAuditable>(entity, state);
        }
    }
}