using Microsoft.Practices.ServiceLocation;

namespace iQuarc.DataAccess
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IDbContextFactory contextFactory;
        private readonly IInterceptorsResolver interceptorsResolver;

        public UnitOfWorkFactory()
            :this(ServiceLocator.Current)
        {
        }

        public UnitOfWorkFactory(IServiceLocator serviceLocator)
        {
            contextFactory = serviceLocator.GetInstance<IDbContextFactory>();
            interceptorsResolver = serviceLocator.GetInstance<IInterceptorsResolver>();
        }

        public UnitOfWorkFactory(IDbContextFactory contextFactory, IInterceptorsResolver interceptorsResolver)
        {
            this.contextFactory = contextFactory;
            this.interceptorsResolver = interceptorsResolver;
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork(contextFactory, interceptorsResolver);
        }
    }
}