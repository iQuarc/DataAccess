using System.Data.Entity;

namespace iQuarc.Finance.DataAccess
{
	public class DbContextFactory<T> : IDbContextFactory where T : DbContext, new()
	{
		public DbContext CreateContext()
		{
			return new T();
		}
	}
}