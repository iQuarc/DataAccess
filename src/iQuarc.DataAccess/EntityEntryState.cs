using System;
using System.Data.Entity;

namespace iQuarc.DataAccess
{
	[Flags]
	public enum EntityEntryState
	{
		Detached = EntityState.Detached,
		Unchanged = EntityState.Unchanged,
		Added = EntityState.Added,
		Deleted = EntityState.Deleted,
		Modified = EntityState.Modified,
	}
}