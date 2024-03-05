using System.Collections.Concurrent;
using BookStore.Models;
using BookStore.Services.Classes;
using BookStore.Services.Interfaces;

namespace UnitTests.ServicesTests;

[TestClass]
public class InventoryServiceTests
{
    internal static ConcurrentDictionary<string, int> DummyInventory()
    {
        ConcurrentDictionary<string, int> i = new();
        i.TryAdd("Red Book", 100);
        i.TryAdd("Green Book", 100);
        i.TryAdd("Blue Book", 100);
        i.TryAdd("Black Book", 100);
        i.TryAdd("White Book", 100);
        return i;
    }
    
    [TestMethod]
    public void InitializeInventory()
    {
        InventoryService inv = new(DummyInventory());
        inv.PrintInventory();
    }

    [TestMethod]
    public void AllocateOrder()
    {
        InventoryService inv = new(DummyInventory());
        OrderLine order = new("Red Book", 10, 10);
        inv.AllocateLine(order);
        inv.PrintInventory();
        Assert.AreEqual(90, inv.GetItemInventory("Red Book"));
    }
    
    [TestMethod]
    public void AllocateBigOrder()
    {
        InventoryService inv = new(DummyInventory());
        OrderLine order = new("Red Book", 110, 10);
        inv.AllocateLine(order);
        inv.PrintInventory();
        // Assert.AreEqual(100, order.Allocated);
        Assert.AreEqual(0, inv.GetItemInventory("Red Book"));
    }

    [TestMethod]
    public void CancelBigOrder()
    {
        InventoryService inv = new(DummyInventory());
        OrderLine order = new("Red Book", 110, 10);
        inv.AllocateLine(order);
        inv.CancelLine(order);
        inv.PrintInventory();
        Assert.AreEqual(100, inv.GetItemInventory("Red Book"));
    }
}