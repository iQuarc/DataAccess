using iQuarc.DataAccess.UnitTests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iQuarc.DataAccess.UnitTests
{
    [TestClass]
    public  class RepositoryTests : RepositoryBaseTests
    {
        protected override IRepository GetTarget(IDbContextFactory factory, IInterceptorsResolver resolver)
        {
            return new Repository(factory, resolver, new ContextUtilitiesDouble());
        }
    }
}