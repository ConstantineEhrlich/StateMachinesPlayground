using Stateless;

namespace BookStore.Models;

public class OrderLine
{
    private enum Trigger
    {
        Allocate,
        Cancel
    }
    private enum State
    {
        Unallocated,
        Allocated,
        InsufficientInventory,
        Cancelled
    }
    
    private readonly StateMachine<State, Trigger> _machine;
    private readonly StateMachine<State, Trigger>.TriggerWithParameters<bool> _allocateTrigger;
    
    public string? BookId { get; set; }
    public int Ordered { get; set; }
    public decimal Price { get; set; }

    public string OrderStatus => _machine.State.ToString();


    public OrderLine()
    {
        _machine = new StateMachine<State, Trigger>(State.Unallocated);
        _allocateTrigger = _machine.SetTriggerParameters<bool>(Trigger.Allocate);
        
        
        _machine.Configure(State.Unallocated)
            .Permit(Trigger.Allocate, State.Allocated)
            .Permit(Trigger.Allocate, State.InsufficientInventory)
            .Permit(Trigger.Cancel, State.Cancelled);

        _machine.Configure(State.Allocated)
            .Permit(Trigger.Cancel, State.Cancelled);
        
    }
    
    
    public OrderLine(string bookId, int ordered, decimal price)
    {
        BookId = bookId;
        Ordered = ordered;
        Price = price;
        
    }
    
    
    
    
    
}