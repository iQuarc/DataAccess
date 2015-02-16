using System;

namespace iQuarc.DataAccess
{
    public interface IExceptionHandler
	{
		void Handle(Exception exception);
	}
}