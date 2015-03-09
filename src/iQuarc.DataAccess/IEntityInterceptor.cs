namespace iQuarc.DataAccess
{
	/// <summary>
	///     Defines an interceptor for a specific entity type.
	///     Any implementation registered into the Service Locator container with this interface as contract will be applied to
	///     entities of type T only
	/// </summary>
	/// <typeparam name="T">The type of the entities this interceptor will intercept</typeparam>
	public interface IEntityInterceptor<T> : IEntityInterceptor
		where T : class
	{
		/// <summary>
		///     Logic to execute after the entity was read from the database
		/// </summary>
		/// <param name="entry">The entry that was read</param>
		/// <param name="repository">A reference to the repository that read this entry. It may be used to read additional data.</param>
		void OnLoad(IEntityEntry<T> entry, IRepository repository);

		/// <summary>
		///     Logic to execute before the entity is written into the database. This runs in the same transaction with the Save
		///     operation.
		///     This applies to Add, Update or Insert operation
		/// </summary>
		/// <param name="entry">The entity being saved</param>
		/// <param name="repository">A reference to the repository that read this entry. It may be used to read additional data.</param>
		void OnSave(IEntityEntry<T> entry, IRepository repository);

		/// <summary>
		///     Logic to execute before the entity is deleted the database. This runs in the same transaction with the Save
		///     operation.
		/// </summary>
		/// <param name="entry">The entity being deleted</param>
		/// <param name="repository">A reference to the repository that read this entry. It may be used to read additional data.</param>
		void OnDelete(IEntityEntry<T> entry, IRepository repository);
	}

	/// <summary>
	///     Defines a global entity interceptor.
	///     Any implementation registered into the Service Locator container with this interface as contract will be applied to
	///     all entities of any type
	/// </summary>
	public interface IEntityInterceptor
	{
		/// <summary>
		///     Logic to execute after the entity was read from the database
		/// </summary>
		/// <param name="entry">The entry that was read</param>
		/// <param name="repository">A reference to the repository that read this entry. It may be used to read additional data.</param>
		void OnLoad(IEntityEntry entry, IRepository repository);

		/// <summary>
		///     Logic to execute before the entity is written into the database. This runs in the same transaction with the Save
		///     operation.
		///     This applies to Add, Update or Insert operation
		/// </summary>
		/// <param name="entry">The entity being saved</param>
		/// <param name="repository">A reference to the repository that read this entry. It may be used to read additional data.</param>
		void OnSave(IEntityEntry entry, IRepository repository);

		/// <summary>
		///     Logic to execute before the entity is deleted the database. This runs in the same transaction with the Save
		///     operation.
		/// </summary>
		/// <param name="entry">The entity being deleted</param>
		/// <param name="repository">A reference to the repository that read this entry. It may be used to read additional data.</param>
		void OnDelete(IEntityEntry entry, IRepository repository);
	}
}