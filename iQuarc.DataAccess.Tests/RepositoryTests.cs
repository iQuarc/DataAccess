using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iQuarc.DataAccess.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        [TestMethod]
        public void GetEntities_FilterById_ReturnsOneRecord()
        {
            Repository repository = GetTarget();
            IQueryable<User> users = repository.GetEntities<User>();
            User actual = users.FirstOrDefault(x => x.Id == 2);

            Assert.IsNotNull(actual);
        }

        private static Repository GetTarget()
        {
            Mock<IInterceptorsResolver> resolver = new Mock<IInterceptorsResolver>();
            resolver.Setup(x => x.GetEntityInterceptors(It.IsAny<Type>()))
                    .Returns(new IEntityInterceptor[0]);
            resolver.Setup(x => x.GetGlobalInterceptors())
                    .Returns(new IEntityInterceptor[0]);

            Mock<IDbContextFactory> factory = new Mock<IDbContextFactory>();
            factory.Setup(x => x.CreateContext(It.IsAny<Action<object>>()))
                   .Returns(CreateContext());

            Repository repository = new Repository(resolver.Object, factory.Object);
            return repository;
        }

        private static DbContext CreateContext()
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

            Mock<DbContext> context = new Mock<DbContext>();

            DbSet<User> userSet = users.MockDbSet();
            DbSet<Role> roleSet = roles.MockDbSet();

            context.Setup(x => x.Set<User>()).Returns(() => userSet);
            context.Setup(x => x.Set<Role>()).Returns(() => roleSet);

            return context.Object;
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }

    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<User> Users { get; set; }
    }
}