using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace iQuarc.DataAccess
{
    public class DbContextFactory<T> : IDbContextFactory where T : DbContext, new()
    {
        public DbContext CreateContext(Action<object> onEntityLoaded)
        {
            T context = new T();

            ObjectContext objectContext = ((IObjectContextAdapter) context).ObjectContext;
            //TODO how about detach?
            objectContext.ObjectMaterialized += (sender, args) => onEntityLoaded(args.Entity);

            return context;
        }
    }
}