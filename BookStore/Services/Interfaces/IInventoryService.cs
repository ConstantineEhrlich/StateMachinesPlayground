using BookStore.Models;

namespace BookStore.Services.Interfaces;

public interface IInventoryService
{
    void Allocate(OrderLine order);
    void Cancel(OrderLine order);
}