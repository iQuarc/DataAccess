using System.Collections.Generic;
using System.Data.Entity;
using Moq;

namespace iQuarc.DataAccess.Tests
{
    static class DbUtilities
    {
        public static DbSet<T> MockDbSet<T>(this IList<T> values) where T : class
        {
            return new FakeSet<T>(values);
        }

        public static IDbContextFactory BuildFactoryStub(this DbContext contextStub)
        {
            Mock<IDbContextFactory> contextFactoryStub = new Mock<IDbContextFactory>();
            contextFactoryStub.Setup(s => s.CreateContext())
                              .Returns(new DbContextFakeWrapper(contextStub));
            return contextFactoryStub.Object;
        }
    }
}