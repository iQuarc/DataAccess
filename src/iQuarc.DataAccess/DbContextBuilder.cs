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

        private IDbContextWrapper contextWrapper;
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
                if (contextWrapper == null)
                    Init();

                return contextWrapper.Context;
            }
        }

        private void Init()
        {
            globalInterceptors = interceptorsResolver.GetGlobalInterceptors();
            
            contextWrapper = factory.CreateContext();
            contextWrapper.ObjectMaterialized += contextWrapper_ObjectMaterialized;
        }

        void contextWrapper_ObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            InterceptLoad(globalInterceptors, e.Entity);

            Type entityType = ObjectContext.GetObjectType(e.Entity.GetType());
            IEnumerable<IEntityInterceptor> entityInterceptors = interceptorsResolver.GetEntityInterceptors(entityType);
            InterceptLoad(entityInterceptors, e.Entity);
        }

        private void InterceptLoad(IEnumerable<IEntityInterceptor> interceptors, object entity)
        {
            foreach (var interceptor in interceptors)
            {
                DbEntityEntry dbEntry = Context.Entry(entity);
                EntityEntry entry = new EntityEntry(dbEntry);
                interceptor.OnLoad(entry, repository);
            }
        }

        public void Dispose()
        {
            if (contextWrapper != null)
            {
                contextWrapper.ObjectMaterialized -= contextWrapper_ObjectMaterialized;
                contextWrapper.Dispose();
            }
        }
    }
}