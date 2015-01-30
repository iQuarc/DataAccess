using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace iQuarc.DataAccess
{
    sealed class UnitOfWork : IUnitOfWork
    {
        private readonly IEnumerable<IEntityInterceptor> globalInterceptors;
        private readonly IInterceptorsResolver interceptorsResolver;

        private readonly DbContextBuilder contextBuilder;
        private TransactionScope transactionScope;

        private static readonly IRepositoryExceptionHandler exceptionHandlers = new RepositoryExceptionHandler();

        internal UnitOfWork(IInterceptorsResolver interceptorsResolver, IDbContextFactory contextFactory)
        {
            this.interceptorsResolver = interceptorsResolver;
            this.globalInterceptors = interceptorsResolver.GetGlobalInterceptors();

            contextBuilder = new DbContextBuilder(contextFactory, interceptorsResolver, this);
        }


        public IQueryable<T> GetEntities<T>() where T : class
        {
            return contextBuilder.Context.Set<T>();
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            return this;
        }

        public void SaveChanges()
        {
            try
            {
                InterceptSave(new List<object>());

                contextBuilder.Context.SaveChanges();

                if (transactionScope != null)
                    transactionScope.Complete();
            }
            catch (Exception e)
            {
                Handle(e);
            }
        }

        private static void Handle(Exception exception)
        {
            exceptionHandlers.Handle(exception);
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                InterceptSave(new List<object>());

                await contextBuilder.Context.SaveChangesAsync();

                if (transactionScope != null)
                    transactionScope.Complete();
            }
            catch (Exception e)
            {
                Handle(e);
            }
        }

        private void InterceptSave(List<object> interceptedEntities)
        {
            IEnumerable<object> modifiedEntities = GetInterceptorModifiedEntities(contextBuilder.Context)
                .Select(e => e.Entity).ToList();

            if (modifiedEntities.All(interceptedEntities.Contains))
                return;

            foreach (object entity in modifiedEntities.Where(e => !interceptedEntities.Contains(e)))
            {
                InterceptSave(this.globalInterceptors, entity);

                Type entityType = ObjectContext.GetObjectType(entity.GetType());
                IEnumerable<IEntityInterceptor> interceptors = interceptorsResolver.GetEntityInterceptors(entityType);

                this.InterceptSave(interceptors, entity);
                interceptedEntities.AddIfNotExists(entity);
            }

            InterceptSave(interceptedEntities);
        }

        private void InterceptSave(IEnumerable<IEntityInterceptor> interceptors, object entity)
        {
            foreach (var interceptor in interceptors)
            {
                DbEntityEntry dbEntry = contextBuilder.Context.Entry(entity);
                EntityEntry entry = new EntityEntry(dbEntry);

                interceptor.OnSave(entry, this);
            }
        }

        private static IEnumerable<DbEntityEntry> GetInterceptorModifiedEntities(DbContext context)
        {
            context.ChangeTracker.DetectChanges();
            var modifiedEntities = context.ChangeTracker.Entries()
                                          .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            return modifiedEntities;
        }

        public void Add<T>(T entity) where T : class
        {
            contextBuilder.Context.Set<T>().Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            contextBuilder.Context.Set<T>().Remove(entity);
        }

        public void BeginTransactionScope(SimplifiedIsolationLevel isolationLevel)
        {
            if (transactionScope != null)
                throw new InvalidOperationException("Cannot begin another transaction scope");

            transactionScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions {IsolationLevel = (IsolationLevel) isolationLevel});
        }

        public void Dispose()
        {
            if (transactionScope != null)
                transactionScope.Dispose();

            if (contextBuilder != null)
                contextBuilder.Dispose();
        }
    }
}