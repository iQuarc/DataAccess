using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iQuarc.DataAccess.UnitTests
{
    [TestClass]
    public class ExceptionHandlerTests
    {
        [TestMethod]
        public void Handle_ExceptionWhichNoSpecificHandlerCanHandle_RepositoryViolationExceptionThrown()
        {
            ExceptionHandler handler = new ExceptionHandler();
            Exception e = new Exception();

            Action act = () => handler.Handle(e);

            act.ShouldThrow<RepositoryViolationException>(actual => ReferenceEquals(actual.InnerException, e));
        }
    }
}