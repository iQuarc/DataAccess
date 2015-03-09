using System;
using iQuarc.DataAccess.UnitTests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iQuarc.DataAccess.UnitTests
{
    [TestClass]
    public class AuditableInterceptorBaseTests
    {
        private readonly IRepository repDummy = new Mock<IRepository>().Object;

        [TestMethod]
        public void OnSave_EntityModified_CreationDateSet()
        {
            DateTime previousEditTime = DateTime.Now.AddDays(-1);
            Order order = new Order {LastEditDate = previousEditTime};

            var target = GetTarget("any user name");

			target.OnSave(order.AsAuditableEntry(EntityEntryState.Modified), repDummy);

            Assert.IsTrue(order.LastEditDate > previousEditTime, "Last edit date not set, but expected");
        }

        [TestMethod]
        public void OnSave_EntityModified_EditBySet()
        {
            Order order = new Order();
            var target = GetTarget("John");

			target.OnSave(order.AsAuditableEntry(EntityEntryState.Modified), repDummy);

            Assert.AreEqual("John", order.LastEditBy);
        }

        [TestMethod]
        public void OnSave_EntityAdded_CreationDateSet()
        {
            Order order = new Order();
            var target = GetTarget("any user name");

			target.OnSave(order.AsAuditableEntry(EntityEntryState.Added), repDummy);

            Assert.IsTrue(order.CreationDate > DateTime.MinValue, "Creation date not set, but expected");
        }

        [TestMethod]
        public void OnSave_EntityAdded_CreatedBySet()
        {
            Order order = new Order();
            var target = GetTarget(currentUserName: "John");

			target.OnSave(order.AsAuditableEntry(EntityEntryState.Added), repDummy);

            Assert.AreEqual("John", order.CreatedBy);
        }

        private AuditableInterceptorBase GetTarget(string currentUserName)
        {
            return new TestableAuditableInterceptor(currentUserName);
        }

        private class TestableAuditableInterceptor : AuditableInterceptorBase
        {
            private string userName;

            public TestableAuditableInterceptor(string userName)
            {
                this.userName = userName;
            }

            protected override string GetCurrentUserName()
            {
                return userName;
            }
        }

        private class Order : IAuditable
        {
            public int Id { get; set; }
            public string CustomerName { get; set; }

            public DateTime? LastEditDate { get; set; }
            public DateTime CreationDate { get; set; }
            public string LastEditBy { get; set; }
            public string CreatedBy { get; set; }
        }
    }
}