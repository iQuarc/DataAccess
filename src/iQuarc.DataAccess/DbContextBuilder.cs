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
        private readonly IDbContextUtilities contextUtilities;

        private IDbContextWrapper contextWrapper;
        private IEnumerable<IEntityInterceptor> globalInterceptors;

        public DbContextBuilder(IDbContextFactory factory, IInterceptorsResolver interceptorsResolver, IRepository repository)
            :this(factory,interceptorsResolver,repository,new DbContextUtilities())
        {
        }

        public DbContextBuilder(IDbContextFactory factory, IInterceptorsResolver interceptorsResolver, IRepository repository, IDbContextUtilities contextUtilities)
        {
            this.factory = factory;
            this.interceptorsResolver = interceptorsResolver;
            this.repository = repository;
            this.contextUtilities = contextUtilities;
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
            contextWrapper.EntityLoaded += ContextWrapperEntityLoaded;
        }

        private void ContextWrapperEntityLoaded(object sender, EntityLoadedEventHandlerArgs e)
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
                IEntityEntry entry = contextUtilities.GetEntry(entity, Context);
                interceptor.OnLoad(entry, repository);
            }
        }

        public void Dispose()
        {
            if (contextWrapper != null)
            {
                contextWrapper.EntityLoaded -= ContextWrapperEntityLoaded;
                contextWrapper.Dispose();
            }
        }
    }
}