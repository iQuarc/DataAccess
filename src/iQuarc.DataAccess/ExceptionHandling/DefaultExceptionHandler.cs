using System;

namespace iQuarc.DataAccess
{
	internal class DefaultExceptionHandler : IExceptionHandler
	{
		public void Handle(Exception exception)
		{
			throw new RepositoryViolationException(exception);
		}
	}
}