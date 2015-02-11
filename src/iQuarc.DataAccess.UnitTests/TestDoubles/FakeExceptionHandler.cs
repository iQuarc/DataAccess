using System;

namespace iQuarc.DataAccess.UnitTests.TestDoubles
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