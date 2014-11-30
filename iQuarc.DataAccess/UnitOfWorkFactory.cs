using System;
using iQuarc.AppBoot;

namespace iQuarc.DataAccess
{
	[Service(typeof(IUnitOfWorkFactory), Lifetime.AlwaysNew)]
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