using System.Linq;

namespace iQuarc.DataAccess
{
	/// <summary>
	/// Generic repository contract for database operations.
	/// </summary>
	public interface IRepository
	{
		/// <summary>
		/// Gets the entities from the database.
		/// </summary>
		/// <typeparam name="T">The type of the entities to be retrieved from the database.</typeparam>
		/// <returns>A <see cref="IQueryable"/> for the entities from the database.</returns>
		IQueryable<T> GetEntities<T>() where T : class;

		/// <summary>
		/// Creates a new <see cref="IUnitOfWork"/> to perform database operations.
		/// </summary>
		/// <returns></returns>
		IUnitOfWork CreateUnitOfWork();
	}
}