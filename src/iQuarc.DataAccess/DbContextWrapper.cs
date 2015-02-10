using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace iQuarc.DataAccess
{
    public interface IDbContextWrapper : IDisposable
    {
        DbContext Context { get; }
        event ObjectMaterializedEventHandler ObjectMaterialized;
    }

    public sealed class DbContextWrapper : IDbContextWrapper
    {
        private readonly ObjectContext objectContext;

        public DbContextWrapper(DbContext context)
        {
            Context = context;

            objectContext = ((IObjectContextAdapter) context).ObjectContext;
            objectContext.ObjectMaterialized += ObjectMaterializedInternalHandler;
        }

        private void ObjectMaterializedInternalHandler(object sender, ObjectMaterializedEventArgs e)
        {
            ObjectMaterializedEventHandler handler = ObjectMaterialized;
            if (handler != null)
                handler(this, e);
        }

        public DbContext Context { get; private set; }

        public event ObjectMaterializedEventHandler ObjectMaterialized;
       
        public void Dispose()
        {
            objectContext.ObjectMaterialized -= ObjectMaterializedInternalHandler;
            Context.Dispose();
        }
    }
}