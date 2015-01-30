using System;
using System.Data.Entity;

namespace iQuarc.DataAccess
{
	public interface IDbContextFactory
	{
	    DbContext CreateContext(Action<object> onEntityLoaded );
	}
}