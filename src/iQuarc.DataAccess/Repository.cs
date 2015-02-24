using System;
using System.Data.Entity;
using System.Linq;

namespace iQuarc.DataAccess
{
    /// <summary>
    ///     Implements a repository for reading data with Entity Framework
    ///     The entities retrieved through this repository are not meant to be modified and persisted back.
    ///     This implementation is optimized for read-only operations. For reading data for edit, or delete create and use an IUnitOfWork
    /// </summary>
    public class Repository : IRepository, IDisposable
    {
        private readonly IInterceptorsResolver interceptorsResolver;
        private readonly IDbContextFactory contextFactory;

        private readonly DbContextBuilder contextBuilder;

        public Repository(IDbContextFactory contextFactory, IInterceptorsResolver interceptorsResolver)
            : this(contextFactory, interceptorsResolver, new DbContextUtilities())
        {
        }

        internal Repository(IDbContextFactory contextFactory, IInterceptorsResolver interceptorsResolver, IDbContextUtilities contextUtilities)
        {
            this.interceptorsResolver = interceptorsResolver;
            this.contextFactory = contextFactory;

            contextBuilder = new DbContextBuilder(contextFactory, interceptorsResolver, this, contextUtilities);
        }

        public IQueryable<T> GetEntities<T>() where T : class
        {
            return Context.Set<T>().AsNoTracking();
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork(contextFactory, interceptorsResolver);
        }

        protected DbContext Context
        {
            get { return contextBuilder.Context; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (contextBuilder != null)
                    contextBuilder.Dispose();
            }
        }
    }
}