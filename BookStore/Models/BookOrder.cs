using Stateless;

namespace BookStore.Models;

public class BookOrder
{
    internal StateMachine<BookOrder.State, BookOrder.Trigger>? Machine;
    internal enum Trigger { Process, Cancel }
    public enum State
    {
        Draft,
        LinesApproved,
        InsufficientInventory,
        PaymentApproved,
        PaymentRejected,
        Delivered,
        Returned,
        Cancelled
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



