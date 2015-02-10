using System;
using System.Collections.Generic;
using System.Linq;

namespace iQuarc.DataAccess.Tests
{
    class InterceptorDouble : IEntityInterceptor
    {
        private readonly List<IEntityEntry> onLoad = new List<IEntityEntry>();
        private readonly List<IEntityEntry> onSave = new List<IEntityEntry>();

        public IEnumerable<object> InterceptedOnOnLoad
        {
            get { return onLoad.Select(e=>e.Entity); }
        }

        public IEnumerable<object> InterceptedOnSave
        {
            get { return onSave.Select(e=>e.Entity); }
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
            throw new NotImplementedException();
        }
    }
}