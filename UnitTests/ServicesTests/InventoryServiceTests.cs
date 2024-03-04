using BookStore.Models;
using BookStore.Services.Classes;
using BookStore.Services.Interfaces;

namespace UnitTests.ServicesTests;

[TestClass]
public class InventoryServiceTests
{
    [TestMethod]
    public void TestInventoryAfterAllocation()
    {
        InventoryService inventory = new InventoryService();
        List<OrderLine> order = new()
        {
            new("Red Book", 20, 10m),
        };

        inventory.TryAllocateOrderAsync(order).Wait();
        
        Assert.AreEqual(80, inventory.GetBalanceOfItem("Red Book"));
    }

    [TestMethod]
    public void TestAllocationFailInsufficientInventory()
    {
        InventoryService inventory = new InventoryService();
        List<OrderLine> order = new()
        {
            new("Red Book", 110, 10m),
        };
        Assert.IsFalse(inventory.TryAllocateOrderAsync(order).Result);
    }
    
    [TestMethod]
    public void TestAllocationOfSameItem()
    {
        InventoryService inventory = new InventoryService();
        List<OrderLine> order = new()
        {
            new("Red Book", 10, 10m),
            new("Red Book", 20, 10m),
        };
        inventory.TryAllocateOrderAsync(order).Wait();
        
        Assert.AreEqual(70, inventory.GetBalanceOfItem("Red Book"));
    }
}