using System.Data.Entity;

namespace iQuarc.Finance.DataAccess
{
	public interface IDbContextFactory
	{
		DbContext CreateContext();
	}
}