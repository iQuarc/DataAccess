using System;
using System.Data.Entity.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iQuarc.DataAccess.UnitTests
{
    [TestClass]
    public class DbEntityValidationExceptionHandlerTests : ExceptionHandlersBaseTests<DataValidationException>
    {
        protected override IExceptionHandler GetTarget(Mock<IExceptionHandler> mock)
        {
            return new DbEntityValidationExceptionHandler(mock.Object);
        }

        protected override Exception GetExpectedInnerException()
        {
            return new DbEntityValidationException();
        }
    }
}