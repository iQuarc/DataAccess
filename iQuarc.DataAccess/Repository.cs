using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using iQuarc.AppBoot;

namespace iQuarc.DataAccess
{
	[Service(typeof(IRepository))]
	public class Repository : IRepository, IDisposable
	{
		private readonly IInterceptorsResolver interceptorsResolver;
		private readonly IDbContextFactory contextFactory;
		
		private readonly IEnumerable<IEntityInterceptor> globalInterceptors;
		private DbContext context;

		public Repository(IInterceptorsResolver interceptorsResolver, IDbContextFactory contextFactory)
		{
			this.contextFactory = contextFactory;
			this.interceptorsResolver = interceptorsResolver;

			this.globalInterceptors = interceptorsResolver.GetGlobalInterceptors();
		}

		private static readonly IRepositoryExceptionHandler exceptionHandlers = new RepositoryExceptionHandler();

		public IQueryable<T> GetEntities<T>() where T : class
		{
			return Context.Set<T>().AsNoTracking();
		}

		public IUnitOfWork CreateUnitOfWork()
		{
			return new UnitOfWork(interceptorsResolver, contextFactory);
		}

		public void Dispose()
		{
			if (context != null)
				context.Dispose();
		}

		private DbContext Context
		{
			get
			{
				if (context == null)
					context = CreateContext();
				return context;
			}
		}

		private DbContext CreateContext()
		{
			DbContext dbContext = contextFactory.CreateContext();
			ObjectContext objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
			objectContext.ObjectMaterialized += OnEntityLoaded;

			return dbContext;
		}

		private void OnEntityLoaded(object sender, ObjectMaterializedEventArgs e)
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
				interceptor.OnLoad(entry, this);
			}
		}

		private static void Handle(Exception exception)
		{
			exceptionHandlers.Handle(exception);
		}

		private sealed class UnitOfWork : IUnitOfWork
		{
			private DbContext context;
			private TransactionScope transactionScope;

			private readonly IInterceptorsResolver interceptorsResolver;
			private readonly IDbContextFactory contextFactory;
			private readonly IEnumerable<IEntityInterceptor> globalInterceptors;

			public UnitOfWork(IInterceptorsResolver interceptorsResolver, IDbContextFactory contextFactory)
			{
				this.contextFactory = contextFactory;
				this.interceptorsResolver = interceptorsResolver;

				this.globalInterceptors = interceptorsResolver.GetGlobalInterceptors();
			}


			public IQueryable<T> GetEntities<T>() where T : class
			{
				return Context.Set<T>();
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

					context.SaveChanges();

					if (transactionScope != null)
						transactionScope.Complete();
				}
				catch (Exception e)
				{
					Handle(e);
				}
			}

			public async Task SaveChangesAsync()
			{
				try
				{
					InterceptSave(new List<object>());

					await context.SaveChangesAsync();

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
				IEnumerable<object> modifiedEntities = GetInterceptorModifiedEntities(context).Select(e => e.Entity).ToList();

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
					DbEntityEntry dbEntry = Context.Entry(entity);
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
				Context.Set<T>().Add(entity);
			}

			public void Delete<T>(T entity) where T : class
			{
				Context.Set<T>().Remove(entity);
			}

			public void BeginTransactionScope(SimplifiedIsolationLevel isolationLevel)
			{
				if (transactionScope != null)
					throw new InvalidOperationException("Cannot begin another transaction scope");

				transactionScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = (IsolationLevel)isolationLevel });
			}

			public void Dispose()
			{
				if (transactionScope != null)
					transactionScope.Dispose();

				if (context != null)
					context.Dispose();
			}

			private DbContext Context
			{
				get
				{
					if (context == null)
						context = contextFactory.CreateContext();
					return context;
				}
			}
		}
	}
}
