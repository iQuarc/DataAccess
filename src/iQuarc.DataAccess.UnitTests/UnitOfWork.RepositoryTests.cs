using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iQuarc.DataAccess.Tests
{
    [TestClass]
    public class UnitOfWorkAsRepositoryTests : RepositoryBaseTests
    {
        protected override IRepository GetTarget(IInterceptorsResolver resolver, IDbContextFactory factory)
        {
            return new UnitOfWork(resolver, factory);
        }
    }
}