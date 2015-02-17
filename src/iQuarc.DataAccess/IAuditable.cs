using System;

namespace iQuarc.DataAccess
{
    public interface IAuditable
    {
        DateTime? LastEditDate { get; set; }
        DateTime CreationDate { get; set; }
        string LastEditBy { get; set; }
        string CreatedBy { get; set; }
    }
}