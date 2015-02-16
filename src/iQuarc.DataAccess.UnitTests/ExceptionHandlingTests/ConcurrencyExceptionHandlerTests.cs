using System;
using System.Data.Entity.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iQuarc.DataAccess.UnitTests
{
    [TestClass]
    public class ConcurrencyExceptionHandlerTests : ExceptionHandlersBaseTests<ConcurrencyRepositoryViolationException>
    {
        protected override IExceptionHandler GetTarget(Mock<IExceptionHandler> mock)
        {
            return new ConcurrencyExceptionHandler(mock.Object);
        }

        protected override Exception GetExpectedInnerException()
        {
            return new OptimisticConcurrencyException();
        }
    }
}