using System;
using System.Data.Entity;
using System.Linq;
using iQuarc.AppBoot;

namespace iQuarc.DataAccess
{
    [Service(typeof (IRepository))]
    public class Repository : IRepository, IDisposable
    {
        private readonly IInterceptorsResolver interceptorsResolver;
        private readonly IDbContextFactory contextFactory;

        private readonly DbContextBuilder contextBuilder;

        public Repository(IInterceptorsResolver interceptorsResolver, IDbContextFactory contextFactory)
            :this(interceptorsResolver,contextFactory, new DbContextUtilities())
        { }

        internal Repository(IInterceptorsResolver interceptorsResolver, IDbContextFactory contextFactory, IDbContextUtilities contextUtilities)
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
            return new UnitOfWork(interceptorsResolver, contextFactory);
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