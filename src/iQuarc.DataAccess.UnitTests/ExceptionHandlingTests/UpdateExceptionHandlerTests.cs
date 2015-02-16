using System;
using System.Data.Entity.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iQuarc.DataAccess.UnitTests
{
    [TestClass]
    public class UpdateExceptionHandlerTests : ExceptionHandlersBaseTests<RepositoryUpdateException>
    {
        protected override IExceptionHandler GetTarget(Mock<IExceptionHandler> mock)
        {
            return new UpdateExceptionHandler(mock.Object);
        }

        protected override Exception GetExpectedInnerException()
        {
            return new UpdateException();
        }
    }
}