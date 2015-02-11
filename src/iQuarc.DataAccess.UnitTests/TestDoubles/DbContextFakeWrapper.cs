using System.Data.Entity;
using Moq;

namespace iQuarc.DataAccess.UnitTests.TestDoubles
{
    sealed class DbContextFakeWrapper : IDbContextWrapper
    {
        private readonly Mock<DbContext> contextDouble;

        public DbContextFakeWrapper()
        {
            this.contextDouble = new Mock<DbContext>();
        }

        public DbContextFakeWrapper(Mock<DbContext> dbContext)
        {
            this.contextDouble = dbContext;
        }

        public Mock<DbContext> ContextDouble
        {
            get { return contextDouble; }
        }

        public DbContext Context
        {
            get { return contextDouble.Object; }
        }

        public event EntityLoadedEventHandler EntityLoaded;

        public void RaiseEntityLoaded(EntityLoadedEventHandlerArgs args)
        {
            if (EntityLoaded != null)
                EntityLoaded(this, args);
        }

        public void Dispose()
        {
            WasDisposed = true;
        }

        public bool WasDisposed { get; private set; }
    }
}