using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace iQuarc.DataAccess
{
    sealed class EntityEntry<T> : IEntityEntry<T>
        where T : class
    {
        private readonly DbEntityEntry<T> entry;

        public EntityEntry(DbEntityEntry<T> entry)
        {
            this.entry = entry;
        }

        public T Entity
        {
            get { return entry.Entity; }
        }

        public EntityEntryState State
        {
            get { return (EntityEntryState)entry.State; }
            set { entry.State = (EntityState)value; }
        }

        public object GetOriginalValue(string propertyName)
        {
            return entry.OriginalValues[propertyName];
        }

        public object GetCurrentValue(string propertyName)
        {
            return entry.CurrentValues[propertyName];
        }

        public void SetOriginalValue(string propertyName, object value)
        {
            if (entry.OriginalValues.PropertyNames.Contains(propertyName))
                entry.OriginalValues[propertyName] = value;
        }

        public void Reload()
        {
            entry.Reload();
        }

        public IEnumerable<string> GetProperties()
        {
            return entry.CurrentValues.PropertyNames;
        }

        public IPropertyEntry Property(string name)
        {
            return new PropertyEntry(entry.Property(name));
        }

        private bool Equals(EntityEntry<T> other)
        {
            return Equals(entry, other.entry);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is EntityEntry<T> && Equals((EntityEntry<T>)obj);
        }

        public override int GetHashCode()
        {
            return (entry != null ? entry.GetHashCode() : 0);
        }
    }

    sealed class EntityEntry : IEntityEntry
    {
        private readonly DbEntityEntry entry;

        public EntityEntry(DbEntityEntry entry)
        {
            this.entry = entry;
        }

        public object Entity
        {
            get { return this.entry.Entity; }
        }

        public EntityEntryState State
        {
            get { return (EntityEntryState)entry.State; }
            set { entry.State = (EntityState)value; }
        }

        public object GetOriginalValue(string propertyName)
        {
            return entry.OriginalValues[propertyName];
        }

        public object GetCurrentValue(string propertyName)
        {
            return entry.CurrentValues[propertyName];
        }

        public IEntityEntry<T> Convert<T>() where T : class
        {
            return new EntityEntry<T>(entry.Cast<T>());
        }

        public void SetOriginalValue(string propertyName, object value)
        {
            if (entry.OriginalValues.PropertyNames.Contains(propertyName))
                entry.OriginalValues[propertyName] = value;
        }

        public void Reload()
        {
            entry.Reload();
        }

        public IEnumerable<string> GetProperties()
        {
            return entry.CurrentValues.PropertyNames;
        }

        public IPropertyEntry Property(string name)
        {
            return new PropertyEntry(entry.Property(name));
        }

        private bool Equals(EntityEntry other)
        {
            return Equals(entry, other.entry);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is EntityEntry && Equals((EntityEntry)obj);
        }

        public override int GetHashCode()
        {
            return (entry != null ? entry.GetHashCode() : 0);
        }
    }

    sealed class PropertyEntry : IPropertyEntry
    {
        private readonly DbPropertyEntry entry;

        public PropertyEntry(DbPropertyEntry entry)
        {
            this.entry = entry;
        }

        public string Name
        {
            get { return entry.Name; }
        }
        public object CurentValue
        {
            get { return entry.CurrentValue; }
            set { entry.CurrentValue = value; }
        }
        public object OriginalValue
        {
            get { return entry.OriginalValue; }
            set { entry.OriginalValue = value; }
        }
        public bool IsModified
        {
            get { return entry.IsModified; }
            set { entry.IsModified = value; }
        }
    }
}