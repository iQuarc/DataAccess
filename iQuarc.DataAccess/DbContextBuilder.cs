using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace iQuarc.DataAccess
{
    sealed class DbContextBuilder : IDisposable
    {
        private readonly IDbContextFactory factory;
        private readonly IInterceptorsResolver interceptorsResolver;
        private readonly IRepository repository;

        private DbContext context;
        private IEnumerable<IEntityInterceptor> globalInterceptors;

        public DbContextBuilder(IDbContextFactory factory, IInterceptorsResolver interceptorsResolver, IRepository repository)
        {
            this.factory = factory;
            this.interceptorsResolver = interceptorsResolver;
            this.repository = repository;
        }

        public DbContext Context
        {
            get
            {
                if (context == null)
                    Init();

                return context;
            }
        }

        private void Init()
        {
            globalInterceptors = interceptorsResolver.GetGlobalInterceptors();
            context = factory.CreateContext(OnEntityLoaded);
        }

        private void OnEntityLoaded(object entity)
        {
            InterceptLoad(globalInterceptors, entity);

            Type entityType = ObjectContext.GetObjectType(entity.GetType());
            IEnumerable<IEntityInterceptor> entityInterceptors = interceptorsResolver.GetEntityInterceptors(entityType);
            InterceptLoad(entityInterceptors, entity);
        }

        private void InterceptLoad(IEnumerable<IEntityInterceptor> interceptors, object entity)
        {
            foreach (var interceptor in interceptors)
            {
                DbEntityEntry dbEntry = context.Entry(entity);
                EntityEntry entry = new EntityEntry(dbEntry);
                interceptor.OnLoad(entry, repository);
            }
        }

        public void Dispose()
        {
            if (context != null)
                context.Dispose();
        }
    }
}