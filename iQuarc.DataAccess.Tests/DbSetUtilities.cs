using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace iQuarc.DataAccess.Tests
{
    static class DbSetUtilities
    {
        public static DbSet<T> MockDbSet<T>(this IList<T> values) where T : class
        {
            return new FakeSet<T>(values);
        }

        private class FakeSet<T> : DbSet<T>, IQueryable, IEnumerable<T> where T : class
        {
            private readonly IQueryable<T> queryable;

            public FakeSet(IEnumerable<T> values)
            {
                queryable = values.AsQueryable();
            }

            IQueryProvider IQueryable.Provider
            {
                get { return queryable.Provider; }
            }

            Expression IQueryable.Expression
            {
                get { return queryable.Expression; }
            }

            Type IQueryable.ElementType
            {
                get { return queryable.ElementType; }
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return queryable.GetEnumerator();
            }
        }
    }
}