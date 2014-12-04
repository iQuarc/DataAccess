using System;
using System.Collections.Generic;

namespace iQuarc.DataAccess
{
	public interface IInterceptorsResolver
	{
		IEnumerable<IEntityInterceptor> GetGlobalInterceptors();
		IEnumerable<IEntityInterceptor> GetEntityInterceptors(Type entityType);
	}
}