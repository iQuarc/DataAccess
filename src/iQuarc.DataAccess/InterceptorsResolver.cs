using System;
using System.Collections.Generic;
using System.Linq;
using iQuarc.AppBoot;
using iQuarc.SystemEx.Priority;
using Microsoft.Practices.ServiceLocation;

namespace iQuarc.DataAccess
{
	[Service(typeof(IInterceptorsResolver))]
	internal class InterceptorsResolver : IInterceptorsResolver
	{
		private readonly IServiceLocator servicelocator;
		private static readonly Type interceptorGenericType = typeof(IEntityInterceptor<>);

		public InterceptorsResolver(IServiceLocator servicelocator)
		{
			this.servicelocator = servicelocator;
		}

		public IEnumerable<IEntityInterceptor> GetGlobalInterceptors()
		{
			return servicelocator.GetAllInstances<IEntityInterceptor>().OrderByPriority();
		}

		public IEnumerable<IEntityInterceptor> GetEntityInterceptors(Type entityType)
		{
			Type interceptorType = interceptorGenericType.MakeGenericType(entityType);
			return servicelocator.GetAllInstances(interceptorType).Cast<IEntityInterceptor>();
		}
	}
}