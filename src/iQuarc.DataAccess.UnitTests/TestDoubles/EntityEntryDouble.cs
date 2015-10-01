using System;
using System.Collections.Generic;

namespace iQuarc.DataAccess.UnitTests.TestDoubles
{
	class EntityEntryDouble : IEntityEntry
	{
		public EntityEntryDouble()
		{
		}

		public EntityEntryDouble(object entity, EntityEntryState state)
		{
			Entity = entity;
			State = state;
		}

		public object Entity { get; private set; }
		public EntityEntryState State { get; set; }
		public object GetOriginalValue(string propertyName)
		{
			throw new NotImplementedException();
		}

		public IEntityEntry<T> Convert<T>() where T : class
		{
			throw new NotImplementedException();
		}

		public void SetOriginalValue(string propertyName, object value)
		{
			throw new NotImplementedException();
		}

	    public void Reload()
	    {
	        throw new NotImplementedException();
	    }

	    protected bool Equals(EntityEntryDouble other)
		{
			return Equals(Entity, other.Entity) && State == other.State;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((EntityEntryDouble) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Entity != null ? Entity.GetHashCode() : 0)*397) ^ (int) State;
			}
		}
	}

    class EntityEntryDouble<T> : IEntityEntry<T> where T : class
    {
        public EntityEntryDouble()
        {
        }

        public EntityEntryDouble(T entity, EntityEntryState state)
        {
            Entity = entity;
            State = state;
        }

        public T Entity { get; set; }
        public EntityEntryState State { get; set; }
        public object GetOriginalValue(string propertyName)
        {
            throw new NotImplementedException();
        }

        public void SetOriginalValue(string propertyName, object value)
        {
            throw new NotImplementedException();
        }

        public void Reload()
        {
            throw new NotImplementedException();
        }

        protected bool Equals(EntityEntryDouble<T> other)
	    {
		    return EqualityComparer<T>.Default.Equals(Entity, other.Entity) && State == other.State;
	    }

	    public override bool Equals(object obj)
	    {
		    if (ReferenceEquals(null, obj)) return false;
		    if (ReferenceEquals(this, obj)) return true;
		    if (obj.GetType() != this.GetType()) return false;
		    return Equals((EntityEntryDouble<T>) obj);
	    }

	    public override int GetHashCode()
	    {
		    unchecked
		    {
			    return (EqualityComparer<T>.Default.GetHashCode(Entity)*397) ^ (int) State;
		    }
	    }
    }
}