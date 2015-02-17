using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using iQuarc.DataAccess.UnitTests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iQuarc.DataAccess.UnitTests
{
    [TestClass]
    public class UnitOfWorkSaveTests
    {
        [TestMethod]
        public void SaveChanges_GlobalInterceptors_EntitiesIntercepted()
        {
            ParamTest__SaveChanges_ModifiedEntities_EntitiesIntercepted(
                resolver => resolver.GetGlobalInterceptors());
        }

        [TestMethod]
        public void SaveChanges_EntityInterceptors_EntitiesIntercepted()
        {
            ParamTest__SaveChanges_ModifiedEntities_EntitiesIntercepted(
                resolver => resolver.GetEntityInterceptors(It.IsAny<Type>()));
        }

        private void ParamTest__SaveChanges_ModifiedEntities_EntitiesIntercepted(Expression<Func<IInterceptorsResolver, IEnumerable<IEntityInterceptor>>> getInterceptorsFunction)
        {
            User user1 = new User(1, "John");
            User user2 = new User(2, "Mary");

            InterceptorDouble interceptorMock = new InterceptorDouble();

            Mock<IInterceptorsResolver> interceptorResolverStub = new Mock<IInterceptorsResolver>();
            interceptorResolverStub.Setup(getInterceptorsFunction).Returns(new[] {interceptorMock});

            UnitOfWork uof = GetTargetWith(new[] {user1, user2}, interceptorResolverStub.Object);

            uof.SaveChanges();

            AssertInterceptedOnSave(interceptorMock, user1, user2);
        }

        [TestMethod]
        public void SaveChanges_GlobalInterceptorModifiesOtherEntities_NewModifiedEntityAlsoIntercepted()
        {
            ParamTest__SaveChanges_InterceptorModifiesOtherEntities_NewModifiedEntitiesAlsoIntercepted(
                resolver => resolver.GetGlobalInterceptors());
        }

        [TestMethod]
        public void SaveChanges_EntityInterceptorModifiesOtherEntities_NewModifiedEntityAlsoIntercepted()
        {
            ParamTest__SaveChanges_InterceptorModifiesOtherEntities_NewModifiedEntitiesAlsoIntercepted(
                resolver => resolver.GetEntityInterceptors(It.IsAny<Type>()));
        }

        private void ParamTest__SaveChanges_InterceptorModifiesOtherEntities_NewModifiedEntitiesAlsoIntercepted(
            Expression<Func<IInterceptorsResolver, IEnumerable<IEntityInterceptor>>> getInterceptorsFunction)
        {
            User user1 = new User(1, "John");
            User user2 = new User(2, "Mary");
            ContextUtilitiesDouble contextStub = new ContextUtilitiesDouble();
            contextStub.AddChangedEntities(1, new[] {user1}); // 1st call to GetChangedEntities --> user1
            contextStub.AddChangedEntities(2, new[] {user1, user2}); // 2nd call to GetChangedEntities --> user1 and user2

            InterceptorDouble interceptorMock = new InterceptorDouble();

            Mock<IInterceptorsResolver> interceptorResolverStub = new Mock<IInterceptorsResolver>();
            interceptorResolverStub.Setup(getInterceptorsFunction).Returns(new[] {interceptorMock});


            UnitOfWork uof = GetTargetWith(contextStub, interceptorResolverStub.Object);


            uof.SaveChanges();

            AssertInterceptedOnSave(interceptorMock, user1, user2);
        }

        private static void AssertInterceptedOnSave(InterceptorDouble interceptorMock, params User[] users)
        {
            interceptorMock.AssertIntercepted(d => d.InterceptedOnSave, users, u => u.Name);
        }

        [TestMethod]
        public void SaveChanges_WhenCalled_SaveChangesOnContext()
        {
            Mock<DbContext> contextMock = new Mock<DbContext>();
            UnitOfWork uof = GetTargetWith(contextMock);

            uof.SaveChanges();

            contextMock.Verify(c => c.SaveChanges(), Times.AtLeastOnce);
        }

        [TestMethod]
        public void SaveChanges_InterceptorThrowsException_ExceptionHandlerGetsIt()
        {
            Exception e = new Exception();
            Mock<IEntityInterceptor> interceptorStub = new Mock<IEntityInterceptor>();
            interceptorStub.Setup(i => i.OnSave(It.IsAny<IEntityEntry>(), It.IsAny<IRepository>()))
                           .Throws(e);

            FakeExceptionHandler handler = new FakeExceptionHandler();
            UnitOfWork uof = GetTargetWith(interceptorStub.Object, handler);

            uof.SaveChanges();

            Assert.AreSame(e, handler.Handled);
        }

        [TestMethod]
        public void SaveChanges_ContextSaveThrowsException_ExceptionHandlerGetsIt()
        {
            Exception e = new Exception();
            Mock<DbContext> dbContextStub = new Mock<DbContext>();
            dbContextStub.Setup(c => c.SaveChanges()).Throws(e);

            FakeExceptionHandler handler = new FakeExceptionHandler();
            UnitOfWork uof = GetTargetWith(dbContextStub, handler);

            uof.SaveChanges();

            Assert.AreSame(e, handler.Handled);
        }

        private UnitOfWork GetTargetWith(Mock<DbContext> context, FakeExceptionHandler handler)
        {
            ContextUtilitiesDouble utilitiesStub = new ContextUtilitiesDouble(new[] {new User()});
            Mock<IInterceptorsResolver> resolverStub = new Mock<IInterceptorsResolver>();

            return GetTargetWith(utilitiesStub, resolverStub.Object, context, handler);
        }

        private UnitOfWork GetTargetWith(IEntityInterceptor interceptor, IExceptionHandler handler)
        {
            ContextUtilitiesDouble utilitiesStub = new ContextUtilitiesDouble(new[] {new User()});

            Mock<IInterceptorsResolver> resolverStub = new Mock<IInterceptorsResolver>();
            resolverStub.Setup(r => r.GetGlobalInterceptors()).Returns(new[] {interceptor});

            return GetTargetWith(utilitiesStub, resolverStub.Object, new Mock<DbContext>(), handler);
        }

        private UnitOfWork GetTargetWith(Mock<DbContext> context)
        {
            ContextUtilitiesDouble contextUtilitiesStub = new ContextUtilitiesDouble(Enumerable.Empty<object>());
            return GetTargetWith(contextUtilitiesStub, new Mock<IInterceptorsResolver>().Object, context);
        }

        private UnitOfWork GetTargetWith(IEnumerable<object> changedEntities, IInterceptorsResolver interceptorsResolver)
        {
            ContextUtilitiesDouble contextUtilitiesStub = new ContextUtilitiesDouble(changedEntities);
            return GetTargetWith(contextUtilitiesStub, interceptorsResolver, new Mock<DbContext>());
        }

        private UnitOfWork GetTargetWith(IDbContextUtilities contextUtilities, IInterceptorsResolver interceptorsResolver)
        {
            return GetTargetWith(contextUtilities, interceptorsResolver, new Mock<DbContext>());
        }

        private UnitOfWork GetTargetWith(IDbContextUtilities contextUtilitiesStub, IInterceptorsResolver interceptorsResolver, Mock<DbContext> contextStub)
        {
            IExceptionHandler handler = new Mock<IExceptionHandler>().Object;
            return GetTargetWith(contextUtilitiesStub, interceptorsResolver, contextStub, handler);
        }

        private UnitOfWork GetTargetWith(IDbContextUtilities contextUtilitiesStub, IInterceptorsResolver interceptorsResolver, Mock<DbContext> contextStub, IExceptionHandler handler)
        {
            var contextFactoryStub = contextStub.BuildFactoryStub();
            return new UnitOfWork(contextFactoryStub, interceptorsResolver, contextUtilitiesStub, handler);
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


        private class ContextUtilitiesDouble : IDbContextUtilities
        {
            private readonly Dictionary<int, IEnumerable<object>> changedAtCall = new Dictionary<int, IEnumerable<object>>();
            private int getChangedEntitiesCount = -1;

            public ContextUtilitiesDouble()
                : this(Enumerable.Empty<object>())
            {
            }

            public ContextUtilitiesDouble(IEnumerable<object> changedEntities)
            {
                changedAtCall[0] = changedEntities;
            }

            public void AddChangedEntities(int callCount, IEnumerable<object> changedEntities)
            {
                changedAtCall[callCount - 1] = changedEntities;
            }

            public IEnumerable<object> GetChangedEntities(DbContext context, Predicate<EntityState> statePredicate)
            {
                getChangedEntitiesCount++;

                IEnumerable<object> changedEntities;
                if (changedAtCall.TryGetValue(getChangedEntitiesCount, out changedEntities))
                    return changedEntities;

                return changedAtCall[0];
            }

            public IEntityEntry GetEntry(object entity, DbContext context)
            {
                Mock<IEntityEntry> entryStub = new Mock<IEntityEntry>();
                entryStub.Setup(e => e.Entity).Returns(entity);
                return entryStub.Object;
            }
        }
    }
}