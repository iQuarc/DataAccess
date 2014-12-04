using System;

namespace iQuarc.DataAccess
{
	[Flags]
	public enum EntityEntryStates
	{
		Detached = 1,
		Unchanged = 2,
		Added = 4,
		Deleted = 8,
		Modified = 16,
	}
}