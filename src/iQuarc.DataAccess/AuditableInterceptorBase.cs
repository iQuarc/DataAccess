using System;

namespace iQuarc.DataAccess
{
    /// <summary>
    ///     Provides an easy implementation template for IAuditable interceptor
    /// </summary>
    public abstract class AuditableInterceptorBase : GlobalEntityInterceptor<IAuditable>
    {
        public override sealed void OnSave(IEntityEntry<IAuditable> entry, IRepository repository)
        {
            var systemDate = DateTime.Now;
            var userName = GetCurrentUserName();

            if (entry.State == EntityEntryStates.Added)
            {
                entry.Entity.CreationDate = systemDate;
                entry.Entity.CreatedBy = userName;
            }

            entry.Entity.LastEditDate = systemDate;
            entry.Entity.LastEditBy = userName;
        }

        protected abstract string GetCurrentUserName();

        public override sealed void OnDelete(IEntityEntry<IAuditable> entry, IRepository repository)
        {
        }

        public override sealed void OnLoad(IEntityEntry<IAuditable> entry, IRepository repository)
        {
        }
    }
}