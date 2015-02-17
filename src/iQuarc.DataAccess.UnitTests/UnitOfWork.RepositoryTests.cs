using iQuarc.DataAccess.UnitTests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iQuarc.DataAccess.UnitTests
{
    [TestClass]
    public class UnitOfWorkAsRepositoryTests : RepositoryBaseTests
    {
        protected override IRepository GetTarget(IDbContextFactory factory, IInterceptorsResolver resolver)
        {
            IExceptionHandler handler = new Mock<IExceptionHandler>().Object;
            return new UnitOfWork(factory, resolver, new ContextUtilitiesDouble(), handler);
        }
    }
}