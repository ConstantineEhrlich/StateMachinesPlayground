using BookStore.Models;

namespace BookStore.Services.Interfaces;

public interface IInventoryService
{
    /// <summary>
    /// Allocates inventory according to the order quantities
    /// </summary>
    /// <param name="orderLines">Order line data</param>
    /// <returns>True, if there is enough inventory, false, if the inventory can't be allocated</returns>
    Task<bool> TryAllocateOrderAsync(ICollection<OrderLine> orderLines);

    /// <summary>
    /// Returns the contents of the order to the inventory.
    /// </summary>
    /// <param name="orderLines">Order line data</param>
    Task ReturnOrderAsync(ICollection<OrderLine> orderLines);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    Task<int> CheckInventoryBalanceAsync(string itemId);
}