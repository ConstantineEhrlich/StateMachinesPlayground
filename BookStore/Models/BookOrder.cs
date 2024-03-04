using Stateless;

namespace BookStore.Models;

public class BookOrder
{   
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

    private enum Trigger
    {
        Process,
        Cancel
    }
    
    private readonly StateMachine<State, Trigger> _machine;
    
    public string? OrderId { get; set; }
    public List<OrderLine> OrderLines { get; } = new();
    public PaymentInfo? PaymentInfo { get; set; }
    public string? DeliveryDestination { get; set; }
    public State OrderStatus => _machine.State;

    public void Process() => _machine?.Fire(Trigger.Process);
    public void Cancel() => _machine?.Fire(Trigger.Cancel);
    
    public BookOrder()
    {
        _machine = new(State.Draft);

        _machine.Configure(State.Draft)
            .PermitIf(Trigger.Process, State.LinesApproved, () => OrderLines.All(ol => ol.LineState == OrderLine.State.Allocated));
        
        _machine.Configure(State.LinesApproved)
            

    }
    
}



