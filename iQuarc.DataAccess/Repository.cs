using System;
using System.Data.Entity;
using System.Linq;
using iQuarc.AppBoot;

namespace iQuarc.DataAccess
{
	[Service(typeof(IRepository), Lifetime.Instance)]
	public sealed class Repository : IRepository
	{
		private readonly DbContext context;
		private bool isDisposed;

		public Repository(IDbContextFactory factory)
		{
			this.context = factory.CreateContext();
		}

		public IQueryable<T> GetEntities<T>() where T : class
		{
			CheckDisposed();
			return context.Set<T>().AsNoTracking();
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
	}
}
