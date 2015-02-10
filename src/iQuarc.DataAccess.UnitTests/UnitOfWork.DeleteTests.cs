using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using iQuarc.DataAccess.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iQuarc.DataAccess.Tests
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

        [TestMethod]
        public void Delete_GlobalInterceptor_DeletedEntitiesIntercepted()
        {
            ParamTest__Delete_EntitiesDeleted_DeletedEntitiesIntercepted(
                i => i.GetGlobalInterceptors());
        }

        [TestMethod]
        public void Delete_EntityInterceptor_DeletedEntitiesIntercepted()
        {
            ParamTest__Delete_EntitiesDeleted_DeletedEntitiesIntercepted(
                i => i.GetEntityInterceptors(It.IsAny<Type>()));
        }

        private void ParamTest__Delete_EntitiesDeleted_DeletedEntitiesIntercepted(Expression<Func<IInterceptorsResolver, IEnumerable<IEntityInterceptor>>> getInterceptorsFunction)
        {
            User u1 = new User(1, "John");
            User u2 = new User(2, "Mary");

            InterceptorDouble interceptorMock = new InterceptorDouble();

            Mock<IInterceptorsResolver> resolverMock = new Mock<IInterceptorsResolver>();
            resolverMock.Setup(getInterceptorsFunction).Returns(new[] {interceptorMock});


            UnitOfWork uof = GetTargetForInterceptorTests(new[] {u1, u2}, resolverMock.Object);

            uof.Delete(u1);
            uof.Delete(u2);


            AssertInterceptedOnDelete(interceptorMock, u1, u2);
        }

        private UnitOfWork GetTargetForInterceptorTests(User[] entities, IInterceptorsResolver resolver)
        {
            FakeSet<User> set = new FakeSet<User>(entities);
            
            Mock<IDbContextUtilities> contextUtilities = new Mock<IDbContextUtilities>();
            contextUtilities.Setup(c => c.GetEntry(It.IsAny<object>(), It.IsAny<DbContext>()))
                            .Returns<object, DbContext>((entity, context) =>
                            {
                                Mock<IEntityEntry> eStub = new Mock<IEntityEntry>();
                                eStub.Setup(e => e.Entity).Returns(entity);
                                return eStub.Object;
                            });

            return GetTargetWith(set, resolver, contextUtilities.Object);
        }

        private UnitOfWork GetTargetWith(FakeSet<User> set)
        {
            IInterceptorsResolver interceptorsResolver = new Mock<IInterceptorsResolver>().Object;
            return GetTargetWith(set, interceptorsResolver, new DbContextUtilities());
        }

        private UnitOfWork GetTargetWith(FakeSet<User> set, IInterceptorsResolver interceptorsResolver, IDbContextUtilities contextUtilities)
        {
            DbContext context = GetContextWith(set);
            IDbContextFactory contextFactory = context.BuildFactoryStub();

            IExceptionHandler handler = new Mock<IExceptionHandler>().Object;
            return new UnitOfWork(interceptorsResolver, contextFactory,contextUtilities, handler);
        }

        private DbContext GetContextWith(FakeSet<User> set)
        {
            Mock<DbContext> contextStub = new Mock<DbContext>();
            contextStub.Setup(c => c.Set<User>()).Returns(set);
            return contextStub.Object;
        }

        private static void AssertInterceptedOnDelete(InterceptorDouble interceptorMock, params User[] users)
        {
            interceptorMock.AssertIntercepted(d => d.InterceptedOnDelete, users, u => u.Name);
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