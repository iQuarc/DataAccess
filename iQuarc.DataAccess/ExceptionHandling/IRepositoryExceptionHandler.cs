using System;

namespace iQuarc.DataAccess
{
	internal interface IRepositoryExceptionHandler
	{
		void Handle(Exception exception);
	}
}