using System.Data.Entity;

namespace iQuarc.DataAccess
{
    public class DbContextFactory<T> : IDbContextFactory where T : DbContext, new()
    {
        public IDbContextWrapper CreateContext()
        {
            T context = new T();
            return new DbContextWrapper(context);
        }
    }
}