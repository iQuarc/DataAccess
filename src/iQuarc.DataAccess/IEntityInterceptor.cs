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
        void OnLoad(IEntityEntry<T> entry, IRepository repository);
        void OnSave(IEntityEntry<T> entry, IRepository repository);
        void OnDelete(IEntityEntry<T> entry, IRepository repository);
    }

    /// <summary>
    ///     Defines a global entity interceptor.
    ///     Any implementation registered into the Service Locator container with this interface as contract will be applied to
    ///     all entities of any type
    /// </summary>
    public interface IEntityInterceptor
    {
        void OnLoad(IEntityEntry entry, IRepository repository);
        void OnSave(IEntityEntry entry, IRepository repository);
        void OnDelete(IEntityEntry entry, IRepository repository);
    }
}