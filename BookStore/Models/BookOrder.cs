using Stateless;

namespace BookStore.Models;

public class BookOrder
{
    internal StateMachine<BookOrder.State, BookOrder.Trigger>? Machine;
    internal enum Trigger { Process, Cancel }
    public enum State
    {
        Draft = 0,
        InsufficientInventory = 20,
        LinesApproved = 40,
        PaymentRejected = 60,
        PaymentApproved = 80,
        Returned = 100,
        Delivered = 120,
        Cancelled = 140
    }
   
    public string? OrderId { get; set; }
    public List<OrderLine> OrderLines { get; init; } = new List<OrderLine>();
    public PaymentInfo? PaymentInfo { get; init; }
    public string? DeliveryDestination { get; set; }
    public State OrderStatus => Machine?.State ?? State.Draft;

    public BookOrder()
    {
    }
    
}



