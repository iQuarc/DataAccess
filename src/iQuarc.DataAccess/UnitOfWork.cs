using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace iQuarc.DataAccess
{
    /// <summary>
    ///     Implements an unit of work for modifying, deleting or adding new data with Entity Framework.
    ///     An instance of this class should have a well defined and short scope. It should be disposed once the changes were
    ///     saved into the database
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IEnumerable<IEntityInterceptor> globalInterceptors;
        private readonly IInterceptorsResolver interceptorsResolver;
        private readonly IDbContextUtilities contextUtilities;

        private readonly DbContextBuilder contextBuilder;
        private TransactionScope transactionScope;

        private readonly IExceptionHandler exceptionHandler;

        internal UnitOfWork(IDbContextFactory contextFactory, IInterceptorsResolver interceptorsResolver)
            : this(contextFactory, interceptorsResolver, new DbContextUtilities(), new ExceptionHandler())
        {
        }

        internal UnitOfWork(IDbContextFactory contextFactory, IInterceptorsResolver interceptorsResolver, IDbContextUtilities contextUtilities, IExceptionHandler exceptionHandler)
        {
            this.interceptorsResolver = interceptorsResolver;
            this.contextUtilities = contextUtilities;
            this.globalInterceptors = interceptorsResolver.GetGlobalInterceptors();
            this.exceptionHandler = exceptionHandler;

            contextBuilder = new DbContextBuilder(contextFactory, interceptorsResolver, this, contextUtilities);
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

        private void Handle(Exception exception)
        {
            exceptionHandler.Handle(exception);
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
            IEnumerable<object> modifiedEntities = GetModifiedEntities(contextBuilder.Context).ToList();

            if (modifiedEntities.All(interceptedEntities.Contains))
                return;

            foreach (object entity in modifiedEntities.Where(e => !interceptedEntities.Contains(e)))
            {
                Intercept(globalInterceptors, entity, (i, e) => i.OnSave(e, this));

                Type entityType = ObjectContext.GetObjectType(entity.GetType());
                IEnumerable<IEntityInterceptor> entityInterceptors = interceptorsResolver.GetEntityInterceptors(entityType);

                this.Intercept(entityInterceptors, entity, (i, e) => i.OnSave(e, this));
                interceptedEntities.AddIfNotExists(entity);
            }

            InterceptSave(interceptedEntities);
        }

        private IEnumerable<object> GetModifiedEntities(DbContext context)
        {
            var modifiedEntities = contextUtilities.GetChangedEntities(context,
                s => s == EntityState.Added || s == EntityState.Modified);
            return modifiedEntities;
        }

        public void Add<T>(T entity) where T : class
        {
            contextBuilder.Context.Set<T>().Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            InterceptDelete(entity);

            contextBuilder.Context.Set<T>().Remove(entity);
        }

        private void InterceptDelete(object entity)
        {
            this.Intercept(globalInterceptors, entity, (i, e) => i.OnDelete(e, this));

            Type entityType = ObjectContext.GetObjectType(entity.GetType());
            IEnumerable<IEntityInterceptor> entityInterceptors = this.interceptorsResolver.GetEntityInterceptors(entityType);
            this.Intercept(entityInterceptors, entity, (i, e) => i.OnDelete(e, this));
        }

        private void Intercept<T>(IEnumerable<T> interceptors, object entity, Action<T, IEntityEntry> intercept)
        {
            foreach (var interceptor in interceptors)
            {
                IEntityEntry entry = contextUtilities.GetEntry(entity, contextBuilder.Context);
                intercept(interceptor, entry);
            }
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