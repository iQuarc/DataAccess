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
    public abstract class RepositoryBaseTests
    {
        [TestMethod]
        public void GetEntities_FilterById_ReturnsOneRecord()
        {
            IInterceptorsResolver resolver = GetEmptyInterceptors();
            IDbContextFactory factory = GetFactory();

            IRepository repository = GetTarget(factory, resolver);

            IQueryable<User> users = repository.GetEntities<User>();
            User actual = users.FirstOrDefault(x => x.Id == 2);

            User expected = new User {Id = 2};
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Dispose_ContextUsed_ContextDisposed()
        {
            DbContextFakeWrapper contextFakeWrapper = CreateContextWithTestData();
            IDbContextFactory factory = GetFactory(contextFakeWrapper);
            IInterceptorsResolver resolver = GetEmptyInterceptors();
            
            IRepository repository = GetTarget(factory, resolver);

            var u = repository.GetEntities<User>().First();


            ((IDisposable) repository).Dispose();

            Assert.IsTrue(contextFakeWrapper.WasDisposed);
        }

        [TestMethod]
        public void GetEntities_GlobalInterceptors_LoadedEntitiesIntercepted()
        {
            ParamTest__GetEntities_EntitiesExists_LoadedEntitiesIntercepted(
                r => r.GetGlobalInterceptors());
        }

        private void ParamTest__GetEntities_EntitiesExists_LoadedEntitiesIntercepted(
            Expression<Func<IInterceptorsResolver, IEnumerable<IEntityInterceptor>>> getInterceptorsFunction)
        {
            InterceptorDouble interceptorMock = new InterceptorDouble();
            IInterceptorsResolver resolverStub = GetResolver(getInterceptorsFunction, interceptorMock);

            DbContextFakeWrapper contextStub = CreateContextWithTestData();
            IDbContextFactory factory = GetFactory(contextStub);

            IRepository rep = GetTarget(factory, resolverStub);
            User u = rep.GetEntities<User>().First();

            AssertInterceptedOnLoad(interceptorMock, u);
        }

        private void AssertInterceptedOnLoad(InterceptorDouble interceptorMock, User user)
        {
            interceptorMock.AssertIntercepted(i => i.InterceptedOnOnLoad, new[] {user}, u => u.Name);
        }

        protected abstract IRepository GetTarget(IDbContextFactory factory, IInterceptorsResolver resolver);

        private static IInterceptorsResolver GetResolver(Expression<Func<IInterceptorsResolver, IEnumerable<IEntityInterceptor>>> getInterceptorsFunction,
            InterceptorDouble interceptorMock)
        {
            Mock<IInterceptorsResolver> stub = new Mock<IInterceptorsResolver>();
            stub.Setup(getInterceptorsFunction).Returns(new[] {interceptorMock});
            return stub.Object;
        }

        private IDbContextFactory GetFactory(DbContextFakeWrapper contextStub)
        {
            Mock<IDbContextFactory> factoryStub = new Mock<IDbContextFactory>();
            factoryStub.Setup(f => f.CreateContext()).Returns(contextStub);
            return factoryStub.Object;
        }

        private static IDbContextFactory GetFactory()
        {
            DbContextFakeWrapper fakeWrapper = CreateContextWithTestData();
            return fakeWrapper.ContextDouble.BuildFactoryStub();
        }

        private static IInterceptorsResolver GetEmptyInterceptors()
        {
            Mock<IInterceptorsResolver> resolver = new Mock<IInterceptorsResolver>();
            resolver.Setup(x => x.GetEntityInterceptors(It.IsAny<Type>()))
                    .Returns(new IEntityInterceptor[0]);
            resolver.Setup(x => x.GetGlobalInterceptors())
                    .Returns(new IEntityInterceptor[0]);
            return resolver.Object;
        }

        private static DbContextFakeWrapper CreateContextWithTestData()
        {
            Role roleAdmin = new Role
            {
                Id = 1,
                Name = "Admin",
                Users = new List<User>
                {
                    new User {Id = 1, Name = "John", RoleId = 1},
                    new User {Id = 2, Name = "Alex", RoleId = 1},
                }
            };

            Role roleUser = new Role
            {
                Id = 2,
                Name = "User",
                Users = new List<User>
                {
                    new User {Id = 3, Name = "Matt", RoleId = 1},
                    new User {Id = 4, Name = "Mike", RoleId = 1},
                }
            };

            List<Role> roles = new List<Role> {roleAdmin, roleUser};
            foreach (Role role in roles)
            {
                foreach (User user in role.Users)
                    user.Role = role;
            }

            List<User> users = roles.SelectMany(r => r.Users).ToList();

            DbContextFakeWrapper wrapper = new DbContextFakeWrapper();

            DbSet<User> userSet = users.MockDbSet(wrapper);
            DbSet<Role> roleSet = roles.MockDbSet(wrapper);

            wrapper.ContextDouble.Setup(x => x.Set<User>()).Returns(() => userSet);
            wrapper.ContextDouble.Setup(x => x.Set<Role>()).Returns(() => roleSet);

            return wrapper;
        }

        private class User
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public int RoleId { get; set; }

            public Role Role { get; set; }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((User) obj);
            }

            protected bool Equals(User other)
            {
                return Id == other.Id;
            }

            public override int GetHashCode()
            {
                return Id;
            }
        }

        private class Role
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public IList<User> Users { get; set; }
        }
    }
}