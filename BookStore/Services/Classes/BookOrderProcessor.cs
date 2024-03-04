using BookStore.Models;
using BookStore.Services.Interfaces;
using Stateless;

namespace BookStore.Services.Classes;

public class BookOrderProcessor
{
    private readonly IInventoryService _inventory;
    private readonly IDeliveryService _delivery;
    private readonly IPaymentService _payment;

    
    public BookOrderProcessor(IInventoryService inventory, IDeliveryService delivery, IPaymentService payment)
    {
        _inventory = inventory;
        _delivery = delivery;
        _payment = payment;
    }





    private void ApproveOrderLines(BookOrder order)
    {
        
    }
}