using System;
using System.Data.Entity.Core;
using iQuarc.SystemEx;

namespace iQuarc.DataAccess
{
	internal class RepositoryUpdateExceptionHandler : IRepositoryExceptionHandler
	{
		private readonly IRepositoryExceptionHandler successor;

		public RepositoryUpdateExceptionHandler(IRepositoryExceptionHandler successor)
		{
			this.successor = successor;
		}

		public void Handle(Exception exception)
		{
			var updateException = exception.FirstInner<UpdateException>();
			if (updateException != null)
			{
				throw new RepositoryUpdateException(updateException);
			}

			successor.Handle(exception);
		}
	}
}