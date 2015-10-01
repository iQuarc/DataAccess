using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace iQuarc.DataAccess
{
	interface IDbContextUtilities
	{
		IEnumerable<IEntityEntry> GetChangedEntities(DbContext context, Predicate<EntityState> statePredicate);
		IEntityEntry GetEntry(object entity, DbContext context);
        IEntityEntry<T> GetEntry<T>(T entity, DbContext context) where T:class;
    }

	class DbContextUtilities : IDbContextUtilities
	{
		public IEnumerable<IEntityEntry> GetChangedEntities(DbContext context, Predicate<EntityState> statePredicate)
		{
			context.ChangeTracker.DetectChanges();
			return context.ChangeTracker.Entries()
			              .Where(entry => statePredicate(entry.State))
			              .Select(entity => new EntityEntry(entity));
		}

		public IEntityEntry GetEntry(object entity, DbContext context)
		{
			DbEntityEntry dbEntry = context.Entry(entity);
			return new EntityEntry(dbEntry);
		}

	    public IEntityEntry<T> GetEntry<T>(T entity, DbContext context) where T : class
	    {
	        DbEntityEntry<T> dbEntityEntry = context.Entry(entity);
	        return new EntityEntry<T>(dbEntityEntry);
	    }
	}
}