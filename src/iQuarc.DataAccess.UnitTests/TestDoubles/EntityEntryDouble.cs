using System;

namespace iQuarc.DataAccess.UnitTests.TestDoubles
{
    class EntityEntryDouble<T> : IEntityEntry<T> where T : class
    {
        public EntityEntryDouble()
        {
        }

        public EntityEntryDouble(T entity, EntityEntryStates state)
        {
            Entity = entity;
            State = state;
        }

        public T Entity { get; set; }
        public EntityEntryStates State { get; set; }
        public object GetOriginalValue(string propertyName)
        {
            throw new NotImplementedException();
        }

        public void SetOriginalValue(string propertyName, object value)
        {
            throw new NotImplementedException();
        }
    }
}