using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iQuarc.DataAccess.UnitTests
{
    public abstract class ExceptionHandlersBaseTests<TException>
        where TException : Exception
    {
        [TestMethod]
        public void Handle_InnerIsConcurrencyException_ExceptionWrappedAndThrown()
        {
            Exception innerException = GetExpectedInnerException();
            Exception ex = new Exception(string.Empty, innerException);

            IExceptionHandler target = GetTarget();

            Action act = () => target.Handle(ex);

            act.ShouldThrow<TException>(
                e => ReferenceEquals(e.InnerException, innerException));
        }


        [TestMethod]
        public void Handle_InnerIsNotExpectedException_SuccessorHandles()
        {
            Mock<IExceptionHandler> mock = new Mock<IExceptionHandler>();
            IExceptionHandler target = GetTarget(mock);
            Exception ex = new Exception(string.Empty, new Exception());

            target.Handle(ex);

            mock.Verify(h => h.Handle(ex), Times.Once);
        }

        private IExceptionHandler GetTarget()
        {
            return GetTarget(new Mock<IExceptionHandler>());
        }

        protected abstract IExceptionHandler GetTarget(Mock<IExceptionHandler> mock);
        protected abstract Exception GetExpectedInnerException();
    }
}