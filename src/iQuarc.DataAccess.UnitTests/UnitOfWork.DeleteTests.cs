using System.Data.Entity;
using iQuarc.DataAccess.UnitTests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iQuarc.DataAccess.UnitTests
{
	[TestClass]
	public class UnitOfWorkDeleteTests
	{
		[TestMethod]
		public void Delete_ExistentEntity_EntityRemoved()
		{
			User user = new User();
			FakeSet<User> set = new FakeSet<User> {user};
			UnitOfWork uof = GetTargetWith(set);

			uof.Delete(user);

			Assert.IsFalse(set.Values.Contains(user), "User found, but not expected");
		}

		private UnitOfWork GetTargetWith(FakeSet<User> set)
		{
			IInterceptorsResolver interceptorsResolver = new Mock<IInterceptorsResolver>().Object;
			Mock<DbContext> context = GetContextWith(set);
			IDbContextFactory contextFactory = context.BuildFactoryStub();

			IExceptionHandler handler = new Mock<IExceptionHandler>().Object;
			return new UnitOfWork(contextFactory, interceptorsResolver, new DbContextUtilities(), handler);
		}

		private Mock<DbContext> GetContextWith(FakeSet<User> set)
		{
			Mock<DbContext> contextStub = new Mock<DbContext>();
			contextStub.Setup(c => c.Set<User>()).Returns(set);
			return contextStub;
		}

		private class User
		{
			public User()
			{
			}

			public User(int id, string name)
			{
				Id = id;
				Name = name;
			}

			public int Id { get; set; }
			public string Name { get; set; }
		}
	}
}