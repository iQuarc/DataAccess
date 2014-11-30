namespace iQuarc.DataAccess
{
	public interface IEntityInterceptor
	{
		void OnInsert(IEntityEntry entry);
		void OnUpdate(IEntityEntry entry);
		void OnDelete(IEntityEntry entry);
	}
}