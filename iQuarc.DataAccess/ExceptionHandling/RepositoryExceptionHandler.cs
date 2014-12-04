using System;

namespace iQuarc.DataAccess
{
	internal class RepositoryExceptionHandler : IRepositoryExceptionHandler
	{
		private readonly IRepositoryExceptionHandler chainHead =
			new RepositorySqlExceptionHandler(
				new RepositoryConcurrencyExceptionHandler(
					new RepositoryUpdateExceptionHandler(
						new RepositoryDbEntityValidationExceptionHandler(
							new RepositoryDefaultExceptionHandler())))
				);

		public void Handle(Exception exception)
		{
			chainHead.Handle(exception);
		}
	}
}