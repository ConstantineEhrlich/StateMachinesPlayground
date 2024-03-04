using BookStore.Models;
using BookStore.Services.Classes;
using BookStore.Services.Interfaces;

namespace UnitTests.ServicesTests;

[TestClass]
public class InventoryServiceTests
{
    [TestMethod]
    public void InitializeInventory()
    {
        InventoryService inv = new();
        inv.PrintInventory();
    }

    [TestMethod]
    public void AllocateOrder()
    {
        InventoryService inv = new();
        OrderLine order = new("Red Book", 10, 10);
        inv.Allocate(order);
        inv.PrintInventory();
        Assert.AreEqual(10, order.Allocated);
        Assert.AreEqual(OrderLine.State.Allocated, order.LineState);
    }
    
    [TestMethod]
    public void AllocateBigOrder()
    {
        InventoryService inv = new();
        OrderLine order = new("Red Book", 110, 10);
        inv.Allocate(order);
        inv.PrintInventory();
        Assert.AreEqual(100, order.Allocated);
        Assert.AreEqual(OrderLine.State.PartiallyAllocated, order.LineState);
    }
}