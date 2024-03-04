using Stateless;

namespace BookStore.Models;

public class OrderLine
{
    public enum State
    {
        Updated,
        Allocated,
        PartiallyAllocated,
        Approved,
        Cancelled
    }
    
    private enum Trigger
    {
        Allocate,
        UpdateOrder,
        ApproveOrder,
        Cancel
    }

    private readonly StateMachine<State, Trigger> _machine;
    private readonly StateMachine<State, Trigger>.TriggerWithParameters<int> _allocateTrigger;
    private readonly StateMachine<State, Trigger>.TriggerWithParameters<int> _updateTrigger;

    
    public string? BookId { get; set; }
    public int Ordered { get; private set; }
    public int Allocated { get; private set; }
    public decimal Price { get; set; }
    public State LineState => _machine.State;
    
    
    internal void Allocate(int allocQty) => _machine.Fire(_allocateTrigger, allocQty);
    internal void Cancel() => _machine.Fire(Trigger.Cancel);
    internal void Approve() => _machine.Fire(Trigger.ApproveOrder);
    
    
    public void UpdateOrdered(int updateQty) => _machine.Fire(_updateTrigger, updateQty);
    
    public OrderLine()
    {
        _machine = new StateMachine<State, Trigger>(State.Updated);
        _allocateTrigger = _machine.SetTriggerParameters<int>(Trigger.Allocate);
        _updateTrigger = _machine.SetTriggerParameters<int>(Trigger.UpdateOrder);

        // Initial state - New/Updated:
        _machine.Configure(State.Updated)
            // Allow cancelling:
            .Permit(Trigger.Cancel, State.Cancelled)
            // Define update trigger behavior:
            .OnEntryFrom(_updateTrigger, OnUpdate)
            // Allow re-update
            .PermitReentry(Trigger.UpdateOrder)
            // Allow allocate, set state according to the allocated amt (full or partial):
            .PermitIf(_allocateTrigger, State.Allocated, allocated => allocated == Ordered)
            .PermitIf(_allocateTrigger, State.PartiallyAllocated, allocated => allocated < Ordered);


        // Partially allocated state
        _machine.Configure(State.PartiallyAllocated)
            // Inherit configuration from the updated state (triggers cancel, update, or allocate):
            .SubstateOf(State.Updated)
            // Define allocation trigger behavior
            .OnEntryFrom(_allocateTrigger, OnAllocate)
            // Allow re-allocation:
            .PermitReentry(Trigger.Allocate);

        // Fully-allocated state
        _machine.Configure(State.Allocated)
            // Inherit all from partially allocated
            .SubstateOf(State.PartiallyAllocated)
            // Allow approve (we are sure that Ordered == Allocated because the state = Allocated)
            .Permit(Trigger.ApproveOrder, State.Approved);

    }


    public OrderLine(string bookId, int ordered, decimal price): this()
    {
        BookId = bookId;
        Ordered = ordered;
        Price = price;
    }

    private void OnAllocate(int allocQty)
    {
        Allocated = allocQty;
    }

    private void OnUpdate(int updateQty)
    {
        Ordered = updateQty;
    }
}