using System;

namespace iQuarc.DataAccess.Tests.TestDoubles
{
    public class FakeExceptionHandler : IExceptionHandler
    {
        public void Handle(Exception exception)
        {
            Handled = exception;
        }

        public Exception Handled
        {
            get;
            private set;
        }
    }
}