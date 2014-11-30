namespace iQuarc.Finance.DataAccess
{
	public interface IEntityEntry
	{
		object Entity { get; }
		object GetOriginalValue(string property);
	}
}