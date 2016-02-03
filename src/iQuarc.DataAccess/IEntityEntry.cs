using System.Collections.Generic;

namespace iQuarc.DataAccess
{
	public interface IEntityEntry<out T>
		where T : class
	{
		T Entity { get; }
		EntityEntryState State { get; set; }
		object GetOriginalValue(string propertyName);
	    object GetCurrentValue(string propertyName);
		void SetOriginalValue(string propertyName, object value);
	    void Reload();
        IEnumerable<string> GetProperties();
        IPropertyEntry Property(string name);
    }

	public interface IEntityEntry
	{
		object Entity { get; }
		EntityEntryState State { get; set; }
		object GetOriginalValue(string propertyName);
        object GetCurrentValue(string propertyName);
        IEntityEntry<T> Convert<T>() where T : class;
		void SetOriginalValue(string propertyName, object value);
	    void Reload();
	    IEnumerable<string> GetProperties();
	    IPropertyEntry Property(string name);
	}

    public interface IPropertyEntry
    {
        string Name { get; }
        object CurentValue { get; set; }
        object OriginalValue { get; set; }
        bool IsModified { get; set; }
    }
}