using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iQuarc.DataAccess.Tests
{
    [TestClass]
    public class RepositoryTests : RepositoryBaseTests
    {
        protected override IRepository GetTarget(IInterceptorsResolver resolver, IDbContextFactory factory)
        {
            return new Repository(resolver, factory);
        }
    }
}