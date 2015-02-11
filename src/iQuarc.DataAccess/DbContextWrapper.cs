using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace iQuarc.DataAccess
{
    public sealed class DbContextWrapper : IDbContextWrapper
    {
        private readonly ObjectContext objectContext;

        public DbContextWrapper(DbContext context)
        {
            Context = context;

            objectContext = ((IObjectContextAdapter) context).ObjectContext;
            objectContext.ObjectMaterialized += ObjectMaterializedHandler;
        }

        private void ObjectMaterializedHandler(object sender, ObjectMaterializedEventArgs e)
        {
            EntityLoadedEventHandler handler = EntityLoaded;
            if (handler != null)
                handler(this, new EntityLoadedEventHandlerArgs(e.Entity));
        }

        public DbContext Context { get; private set; }

        public event EntityLoadedEventHandler EntityLoaded;

        public void Dispose()
        {
            objectContext.ObjectMaterialized -= ObjectMaterializedHandler;
            Context.Dispose();
        }
    }
}