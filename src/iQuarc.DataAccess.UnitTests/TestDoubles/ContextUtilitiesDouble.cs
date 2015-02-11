using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moq;

namespace iQuarc.DataAccess.UnitTests.TestDoubles
{
    class ContextUtilitiesDouble : IDbContextUtilities
    {
        public IEnumerable<object> GetChangedEntities(DbContext context, Predicate<EntityState> statePredicate)
        {
            return Enumerable.Empty<object>();
        }

        public IEntityEntry GetEntry(object entity, DbContext context)
        {
            Mock<IEntityEntry> stub = new Mock<IEntityEntry>();
            stub.Setup(e => e.Entity).Returns(entity);
            return stub.Object;
        }
    }
}