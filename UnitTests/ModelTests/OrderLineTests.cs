using BookStore.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ModelTests
{
    [TestClass]
    public class OrderLineTests
    {
        [TestMethod]
        public void TestUpdate()
        {
            var ol = new OrderLine("Red book", 10, 10);
            ol.UpdateOrdered(20);
            Assert.AreEqual(20, ol.Ordered);
        }

        [TestMethod]
        public void TestReUpdate()
        {
            var ol = new OrderLine("Red book", 10, 10);
            ol.UpdateOrdered(20);
            ol.UpdateOrdered(15);
            Assert.AreEqual(15, ol.Ordered);
        }

        [TestMethod]
        public void TestAllocate()
        {
            var ol = new OrderLine("Red book", 10, 10);
            ol.Allocate(10);
            Assert.AreEqual(10, ol.Allocated);
        }
        
        [TestMethod]
        public void TestReAllocate()
        {
            var ol = new OrderLine("Red book", 10, 10);
            ol.Allocate(10);
            ol.Allocate(4);
            Assert.AreEqual(4, ol.Allocated);
        }

        [TestMethod]
        public void TestInitialState()
        {
            var ol = new OrderLine("Red book", 10, 10);
            Assert.AreEqual(OrderLine.State.Updated, ol.LineState);
        }

        [TestMethod]
        public void TestAllocatedState()
        {
            var ol = new OrderLine("Red book", 10, 10);
            ol.Allocate(10);
            Assert.AreEqual(OrderLine.State.Allocated, ol.LineState);
        }


        [TestMethod]
        public void TestPartiallyAllocatedState()
        {
            var ol = new OrderLine("Red book", 10, 10);
            ol.Allocate(5);
            Assert.AreEqual(OrderLine.State.PartiallyAllocated, ol.LineState);
        }

        [TestMethod]
        public void TestUpdateAfterAllocated()
        {
            var ol = new OrderLine("Red book", 10, 10);
            ol.Allocate(5);
            ol.UpdateOrdered(5);
            Assert.AreEqual(OrderLine.State.Updated, ol.LineState);
        }

        [TestMethod]
        public void TestApproveAfterFullyAllocated()
        {
            var ol = new OrderLine("Red book", 10, 10);
            ol.Allocate(10);
            ol.Approve();
            Assert.AreEqual(OrderLine.State.Approved, ol.LineState);
        }

        [TestMethod]
        public void TestApproveFailAfterPartiallyAllocated()
        {
            var ol = new OrderLine("Red book", 10, 10);
            ol.Allocate(5);
            Assert.ThrowsException<InvalidOperationException>(ol.Approve);
        }

        [TestMethod]
        public void TestCancelFromUpdated()
        {
            var ol = new OrderLine("Red book", 10, 10);
            ol.Cancel();
            Assert.AreEqual(OrderLine.State.Cancelled, ol.LineState);
        }
        
        [TestMethod]
        public void TestCancelFromAllocated()
        {
            var ol = new OrderLine("Red book", 10, 10);
            ol.Allocate(10);
            ol.Cancel();
            Assert.AreEqual(OrderLine.State.Cancelled, ol.LineState);
        }

        [TestMethod]
        public void TestCancelFailFromApproved()
        {
            var ol = new OrderLine("Red book", 10, 10);
            ol.Allocate(10);
            ol.Approve();
            Assert.ThrowsException<InvalidOperationException>(ol.Cancel);
        }
    }
}
