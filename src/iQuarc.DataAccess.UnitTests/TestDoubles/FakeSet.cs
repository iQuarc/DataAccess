using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace iQuarc.DataAccess.Tests.TestDoubles
{
    class FakeSet<T> : DbSet<T>, IQueryable, IEnumerable<T> where T : class
    {
        private readonly List<T> values = new List<T>();

        private IQueryable<T> Queryable
        {
            get { return values.AsQueryable(); }
        }

        public FakeSet()
        {
            values = new List<T>();
        }

        public FakeSet(IEnumerable<T> values)
        {
            this.values.AddRange(values);
        }

        IQueryProvider IQueryable.Provider
        {
            get { return Queryable.Provider; }
        }

        Expression IQueryable.Expression
        {
            get { return Queryable.Expression; }
        }

        Type IQueryable.ElementType
        {
            get { return Queryable.ElementType; }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Queryable.GetEnumerator();
        }

        public IList<T> Values
        {
            get { return values; }
        }

        public override T Add(T entity)
        {
            values.Add(entity);
            return entity;
        }

        public override T Remove(T entity)
        {
            values.Remove(entity);
            return entity;
        }
    }
}