using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iQuarc.DataAccess.UnitTests.TestDoubles
{
    class InterceptorDouble : IEntityInterceptor
    {
        private readonly List<IEntityEntry> onLoad = new List<IEntityEntry>();
        private readonly List<IEntityEntry> onSave = new List<IEntityEntry>();
        private readonly List<IEntityEntry> onDelete = new List<IEntityEntry>();

        public IEnumerable<object> InterceptedOnOnLoad
        {
            get { return onLoad.Select(e => e.Entity); }
        }

        public IEnumerable<object> InterceptedOnSave
        {
            get { return onSave.Select(e => e.Entity); }
        }

        public IEnumerable<object> InterceptedOnDelete
        {
            get { return onDelete.Select(e => e.Entity); }
        }

        public void OnLoad(IEntityEntry entry, IRepository repository)
        {
            onLoad.Add(entry);
        }

        public void OnSave(IEntityEntry entity, IRepository repository)
        {
            onSave.Add(entity);
        }

        public void OnDelete(IEntityEntry entity, IRepository repository)
        {
            onDelete.Add(entity);
        }
    }

    static class InterceptorDoubleExtensions
    {
        public static void AssertIntercepted<T>(this InterceptorDouble interceptor, Func<InterceptorDouble, IEnumerable<object>> onFunc, T[] entities, Func<T, string> msgFunc = null )
        {
            if (msgFunc == null)
                msgFunc = arg => arg.ToString();

            StringBuilder errors = new StringBuilder();
            for (int i = 0; i < entities.Length; i++)
            {
                if (!onFunc(interceptor).Contains(entities[i]))
                    errors.AppendLine(string.Format("User Name='{0}' was not intercepted", msgFunc(entities[i])));
            }
            if (errors.Length > 0)
                Assert.Fail(errors.ToString());
        }
    }
}