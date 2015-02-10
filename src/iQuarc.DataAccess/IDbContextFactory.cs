using System;

namespace iQuarc.DataAccess
{
    public interface IDbContextFactory
    {
        IDbContextWrapper CreateContext();
    }
}