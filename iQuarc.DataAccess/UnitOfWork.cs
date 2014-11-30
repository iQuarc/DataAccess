using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using iQuarc.AppBoot;

namespace iQuarc.DataAccess
{
	[Service(typeof(IUnitOfWork), Lifetime.Instance)]
	public sealed class UnitOfWork : IUnitOfWork
	{
		private static readonly List<EntityState> changed = new List<EntityState> { EntityState.Added, EntityState.Deleted, EntityState.Modified };

		private readonly IEntityInterceptor[] interceptors;
		private readonly DbContext context;
		private bool isDisposed;

		public UnitOfWork(IDbContextFactory factory, IEntityInterceptor[] interceptors)
		{
			this.interceptors = interceptors;
			this.context = factory.CreateContext();
		}

		public IQueryable<T> GetEntities<T>() where T : class
		{
			CheckDisposed();
			return context.Set<T>();
		}
		
		public void Add<T>(T entity) where T : class
		{
			CheckDisposed();
			context.Set<T>().Add(entity);
		}

		public void Update<T>(T entity) where T : class
		{
			CheckDisposed();
		}

		public void Delete<T>(T entity) where T : class
		{
			CheckDisposed();
			context.Entry(entity).State = EntityState.Deleted;
		}

		public void SaveChanges()
		{
			CheckDisposed();

			IEnumerable<DbEntityEntry> changes = context.ChangeTracker.Entries()
				.Where(e => changed.Contains(e.State));

			foreach (DbEntityEntry entry in changes)
			{
				foreach (IEntityInterceptor interceptor in interceptors)
				{
					EntityFrameworkEntry theEntry = new EntityFrameworkEntry(entry);

					switch (entry.State)
					{
						case EntityState.Deleted:
							interceptor.OnDelete(theEntry);
							break;
						case EntityState.Modified:
							interceptor.OnUpdate(theEntry);
							break;
						default:
							interceptor.OnInsert(theEntry);
							break;
					}
				}
			}

			context.SaveChanges();
		}

		public IEnumerable<DbEntityEntry> GetChanges()
		{
			CheckDisposed();

			IEnumerable<DbEntityEntry> changes = context.ChangeTracker.Entries()
				.Where(e => changed.Contains(e.State));

			return changes;
		}

		public void Dispose()
		{
			if (isDisposed) return;

			context.Dispose();
			isDisposed = true;
		}

		private void CheckDisposed()
		{
			if (isDisposed)
				throw new ObjectDisposedException("Repository");
		}

		private class EntityFrameworkEntry : IEntityEntry
		{
			private readonly DbEntityEntry entry;

			public EntityFrameworkEntry(DbEntityEntry entry)
			{
				this.entry = entry;
			}

			public object Entity
			{
				get { return entry.Entity; }
			}

			public object GetOriginalValue(string property)
			{
				return entry.OriginalValues[property];
			}
		}
	}
}