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
			List<IEntityEntry> modifiedAndNotIntercepted = GetModifiedEntities(contextBuilder.Context)
				.Where(e => !interceptedEntities.Contains(e.Entity)).ToList();

			if (modifiedAndNotIntercepted.Count == 0)
				return;

			foreach (IEntityEntry entry in modifiedAndNotIntercepted)
			{
				object entity = entry.Entity;

				Type entityType = ObjectContext.GetObjectType(entity.GetType());
				IEnumerable<IEntityInterceptor> entityInterceptors = interceptorsResolver.GetEntityInterceptors(entityType);

				if (entry.State == EntityEntryState.Deleted)
				{
					Intercept(globalInterceptors, entity, (i, e) => i.OnDelete(e, this));
					Intercept(entityInterceptors, entity, (i, e) => i.OnDelete(e, this));
				}
				else
				{
					Intercept(globalInterceptors, entity, (i, e) => i.OnSave(e, this));
					Intercept(entityInterceptors, entity, (i, e) => i.OnSave(e, this));
				}

				interceptedEntities.AddIfNotExists(entity);
			}

			InterceptSave(interceptedEntities);
		}

		private IEnumerable<IEntityEntry> GetModifiedEntities(DbContext context)
		{
			IEnumerable<IEntityEntry> modifiedEntities = contextUtilities.GetChangedEntities(context,
				s => s == EntityState.Added || s == EntityState.Modified || s == EntityState.Deleted);
			return modifiedEntities;
		}

		private void Intercept<T>(IEnumerable<T> interceptors, object entity, Action<T, IEntityEntry> intercept)
		{
			IEntityEntry entry = contextUtilities.GetEntry(entity, contextBuilder.Context);
			foreach (var interceptor in interceptors)
			{
				intercept(interceptor, entry);
			}
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
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (transactionScope != null)
					transactionScope.Dispose();

				if (contextBuilder != null)
					contextBuilder.Dispose();
			}
		}
	}
}