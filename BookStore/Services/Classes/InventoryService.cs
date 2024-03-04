using System.Collections.Concurrent;
using BookStore.Models;
using BookStore.Services.Interfaces;

namespace BookStore.Services.Classes;

public class InventoryService: IInventoryService
{
    private readonly ConcurrentDictionary<string, int> _inventory;

    public InventoryService()
    {
        _inventory = InitializeInventory();
    }
    
    public void Allocate(OrderLine order)
    {
        if (order.BookId is null)
            throw new NullReferenceException($"{nameof(order.BookId)} is null!");
        
        // First return whatever is allocated already
        AddToInventory(order.BookId, order.Allocated);
        // Allocate minimum between ordered and available
        order.Allocate(Math.Min(order.Ordered, _inventory.GetOrAdd(order.BookId, _ => 0)));
        // Reduce the allocated amount from the inventory
        TakeFromInventory(order.BookId, order.Allocated);
    }

    public void Cancel(OrderLine order)
    {
        if (order.BookId is null)
            throw new NullReferenceException($"{nameof(order.BookId)} is null!");
        
        // Return any allocated inventory
        AddToInventory(order.BookId, order.Allocated);
        order.Cancel();
    }
    
    public void PrintInventory()
    {
        if (!_inventory.Any())
        {
            Console.WriteLine("The inventory is empty.");
            return;
        }

        string hdrItem = "Item";
        string hdrQty = "Quantity";

        // Find the maximum length of the key to align the table
        int maxKeyLength = 1 + Math.Max(_inventory.Keys.Max(k => k.Length), hdrItem.Length);
        int maxQuantityLength = 1 + Math.Max(_inventory.Values.Max().ToString().Length, hdrQty.Length);

        // Create the format string for rows
        string format = $"| {{0,-{maxKeyLength}}} | {{1,{maxQuantityLength}}} |";

        // Print table header
        Console.WriteLine(format, hdrItem, hdrQty);
        Console.WriteLine(new string('-', maxKeyLength + maxQuantityLength + 7));

        // Print each item
        foreach (var kvp in _inventory)
        {
            Console.WriteLine(format, kvp.Key, kvp.Value);
        }
    }


    private void AddToInventory(string itemId, int addQty)
    {
        int currentQty = _inventory.GetOrAdd(itemId, _ => 0);
        _inventory.AddOrUpdate(itemId, currentQty + addQty, (key, oldVal) => oldVal + addQty);
    }

    private void TakeFromInventory(string itemId, int takeQty)
    {
        AddToInventory(itemId, -1 * takeQty);
    }
    
    private static ConcurrentDictionary<string, int> InitializeInventory()
    {
        ConcurrentDictionary<string, int> i = new();
        i.TryAdd("Red Book", 100);
        i.TryAdd("Green Book", 100);
        i.TryAdd("Blue Book", 100);
        i.TryAdd("Black Book", 100);
        i.TryAdd("White Book", 100);
        return i;
    }
}