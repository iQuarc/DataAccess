using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace iQuarc.DataAccess.UnitTests.TestDoubles
{
    class FakeSet<T> : DbSet<T>, IQueryable where T : class
    {
        private readonly DbContextFakeWrapper wrapper;
        private readonly List<T> values = new List<T>();

        private IQueryable<T> queryable; 

        private IQueryable<T> Queryable
        {
            get
            {
                if (queryable == null)
                    queryable = EnumerateAndRaiseEvent().AsQueryable();
                return queryable;
            }
        }

        private IEnumerable<T> EnumerateAndRaiseEvent()
        {
            foreach (var value in values)
            {
                RaiseEntityLoaded(value);
                yield return value;
            }
        }

        public FakeSet()
        {
            values = new List<T>();
        }

        public FakeSet(IEnumerable<T> values)
            : this(values, null)
        {
        }

        public FakeSet(IEnumerable<T> values, DbContextFakeWrapper wrapper)
        {
            this.wrapper = wrapper;
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

        private void RaiseEntityLoaded(T value)
        {
            if (wrapper != null)
            {
                wrapper.RaiseEntityLoaded(new EntityLoadedEventHandlerArgs(value));
            }
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