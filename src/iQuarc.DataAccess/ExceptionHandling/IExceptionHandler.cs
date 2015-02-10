using System;

namespace iQuarc.DataAccess
{
	internal interface IExceptionHandler
	{
		void Handle(Exception exception);
	}
}