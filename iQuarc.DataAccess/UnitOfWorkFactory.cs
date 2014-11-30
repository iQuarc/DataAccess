using System;

namespace iQuarc.DataAccess
{
	public class UnitOfWorkFactory : IUnitOfWorkFactory
	{
		private readonly Func<IUnitOfWork> factory;

		public UnitOfWorkFactory(Func<IUnitOfWork> factory)
		{
			this.factory = factory;
		}

		public IUnitOfWork CreateUnitOfWork()
		{
			IUnitOfWork uow = factory();
			return uow;
		}
	}
}