using System;

namespace iQuarc.DataAccess
{
	internal class RepositoryDefaultExceptionHandler : IRepositoryExceptionHandler
	{
		public void Handle(Exception exception)
		{
			throw new RepositoryViolationException(exception);
		}
	}
}