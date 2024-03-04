using Stateless;

namespace BookStore.Models;

public class BookOrder
{
    private enum State
    {
        Draft,
        PendingInventory,
        InsufficientInventory,
        PendingPayment,
        RejectedPayment,
        PendingShipment,
        Returned,
        Delivered,
        Cancelled,
    }
    
    private enum Trigger
    {
        SubmitOrder,
        CancelOrder,
        InventoryCheckFail,
        InventoryCheckSuccess,
        ChangeOrderLines,
        PaymentCheckFail,
        PaymentCheckSuccess,
        ChangePaymentInfo,
        DeliveryCheckFail,
        DeliveryCheckSuccess,
        ChangeDeliveryDestination,
    }
    
    public string? OrderId { get; set; }
    public ICollection<OrderLine> OrderLines { get; } = new List<OrderLine>();
    public PaymentInfo? PaymentInfo { get; set; }
    private string? DeliveryDestination { get; set; }

    private readonly StateMachine<State, Trigger> _state;
}



