using System;
using System.Data.Entity;

namespace iQuarc.DataAccess
{
    public interface IDbContextWrapper : IDisposable
    {
        DbContext Context { get; }
        event EntityLoadedEventHandler EntityLoaded;
    }

    public delegate void EntityLoadedEventHandler(object sender, EntityLoadedEventHandlerArgs args);

    public class EntityLoadedEventHandlerArgs : EventArgs
    {
        public object Entity { get; private set; }

        public EntityLoadedEventHandlerArgs(object entity)
        {
            Entity = entity;
        }
    }
}