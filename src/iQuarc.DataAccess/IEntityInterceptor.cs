namespace iQuarc.DataAccess
{
	public interface IEntityInterceptor<T> : IEntityInterceptor
		where T : class
	{
		void OnLoad(IEntityEntry<T> entry, IRepository repository);
		void OnSave(IEntityEntry<T> entry, IRepository repository);
		void OnDelete(IEntityEntry<T> entity, IRepository repository);
	}

	public interface IEntityInterceptor
	{
		void OnLoad(IEntityEntry entry, IRepository repository);
		void OnSave(IEntityEntry entity, IRepository repository);
		void OnDelete(IEntityEntry entity, IRepository repository);
	}
}