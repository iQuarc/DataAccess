namespace iQuarc.DataAccess
{
	public interface IUnitOfWorkFactory
	{
		/// <summary>
		/// Creates a new unit of work.
		/// </summary>
		/// <returns></returns>
		IUnitOfWork CreateUnitOfWork();
	}
}