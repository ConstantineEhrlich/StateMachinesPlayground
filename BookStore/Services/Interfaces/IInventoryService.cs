using BookStore.Models;

namespace BookStore.Services.Interfaces;

public interface IInventoryService
{
    void AllocateLine(OrderLine order);
    void CancelLine(OrderLine order);
}