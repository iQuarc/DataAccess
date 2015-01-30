using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moq;

namespace iQuarc.DataAccess.Tests
{
	internal static class DbSetUtilities
	{
		public static Mock<DbSet<T>> MockDbSet<T>(this IList<T> values) where T : class
		{
			IQueryable<T> queryable = values.AsQueryable();
			Mock<DbSet<T>> set = new Mock<DbSet<T>>();

			set.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
			set.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
			set.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
			set.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

			return set;
		}
	}
}