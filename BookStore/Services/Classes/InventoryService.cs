using System.Collections.Concurrent;
using BookStore.Models;
using BookStore.Services.Interfaces;

namespace BookStore.Services.Classes;

public class InventoryService
{
    private ConcurrentDictionary<string, int> _inventory;
    private Random _random;

    public InventoryService()
    {
        _inventory = new();
        InitializeInventory();
        _random = new();
    }
    public async Task<bool> TryAllocateOrderAsync(ICollection<OrderLine> orderLines)
    {
        var order = orderLines.GroupBy(ol => ol.BookId)
            .Select(group => new
            {
                BookId = group.Key,
                Quantity = group.Sum(ol => ol.Ordered),
            }).ToList();
        
        if (order.All(o => GetBalanceOfItem(o.BookId) >= o.Quantity))
        {
            Parallel.ForEach(order, ol => AllocateOrderLine(ol.BookId, ol.Quantity));
        }

        await Task.Delay(0);
        return false;
    }

    public async Task ReturnOrderAsync(ICollection<OrderLine> orderLines)
    {
        await Task.Delay(0);
    }

    public int GetBalanceOfItem(string itemId)
    {
        // Get value of the item, or return zero and add this item to the dictionary with default value
        return _inventory.GetOrAdd(itemId, _ => 0);
    }

    private void AllocateOrderLine(string bookId, int quantity)
    {
        int currentBalance = GetBalanceOfItem(bookId);
        // Compare the current value to the current balance
        // This is not too important since it's executing in the same thread
        _inventory.TryUpdate(bookId, currentBalance -= quantity, currentBalance);
    }

    private void InitializeInventory()
    {
        _inventory.TryAdd("Red Book", 100);
        _inventory.TryAdd("Green Book", 100);
        _inventory.TryAdd("Blue Book", 100);
        _inventory.TryAdd("Black Book", 100);
        _inventory.TryAdd("White Book", 100);
    }
}