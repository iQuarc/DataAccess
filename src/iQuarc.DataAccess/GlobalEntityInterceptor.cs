namespace iQuarc.DataAccess
{
    /// <summary>
    ///     A template to build global entity interceptors.
    ///     Concrete implementation of this should be registered as IEntityInterceptor and they will be applied to all entities
    ///     of any type which inherits T
    /// </summary>
    /// <typeparam name="T">The type which should be inherited / implemented by the entity that is going to be intercepted by this interceptor</typeparam>
    public abstract class GlobalEntityInterceptor<T> : IEntityInterceptor<T>
        where T : class
    {
        public abstract void OnLoad(IEntityEntry<T> entry, IRepository repository);
        public abstract void OnSave(IEntityEntry<T> entry, IUnitOfWork repository);
        public abstract void OnDelete(IEntityEntry<T> entry, IUnitOfWork repository);

        void IEntityInterceptor.OnLoad(IEntityEntry entry, IRepository repository)
        {
            if (entry.Entity is T)
                this.OnLoad(entry.Convert<T>(), repository);
        }

        void IEntityInterceptor.OnSave(IEntityEntry entry, IUnitOfWork repository)
        {
            if (entry.Entity is T)
                this.OnSave(entry.Convert<T>(), repository);
        }

        void IEntityInterceptor.OnDelete(IEntityEntry entry, IUnitOfWork repository)
        {
            if (entry.Entity is T)
                this.OnDelete(entry.Convert<T>(), repository);
        }
    }
}