using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace iQuarc.DataAccess.Tests.TestDoubles
{
    sealed class DbContextFakeWrapper : IDbContextWrapper
    {
        public DbContextFakeWrapper(DbContext dbContext)
        {
            this.Context = dbContext;
        }

        public DbContext Context { get; private set; }
        public event ObjectMaterializedEventHandler ObjectMaterialized;

        public void RaiseObjectMaterialized(ObjectMaterializedEventArgs args)
        {
            if (ObjectMaterialized != null)
                ObjectMaterialized(this, args);
        }

        public void Dispose()
        {
            WasDisposed = true;
        }

        public bool WasDisposed { get; set; }
    }
}