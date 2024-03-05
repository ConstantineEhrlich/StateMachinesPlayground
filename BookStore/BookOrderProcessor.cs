using BookStore.Models;
using BookStore.Services.Interfaces;
using Stateless;

namespace BookStore;

public class BookOrderProcessor
{
    private readonly BookOrder _order;
    private readonly StateMachine<BookOrder.State, BookOrder.Trigger> _machine;
    private readonly IDeliveryService _delSvc;
    private readonly IPaymentService _paySvc;
    private readonly IInventoryService _invSvc;

    public void Process() => _machine.Fire(BookOrder.Trigger.Process);
    public void Cancel() => _machine.Fire(BookOrder.Trigger.Cancel);

    public BookOrderProcessor(BookOrder order, IDeliveryService delSvc, IPaymentService paySvc, IInventoryService invSvc)
    {
        _delSvc = delSvc;
        _paySvc = paySvc;
        _invSvc = invSvc;
        _order = order;
        _machine = new StateMachine<BookOrder.State, BookOrder.Trigger>(_order.OrderStatus);
        _order.Machine = _machine;

        _machine.Configure(BookOrder.State.Draft)
            .PermitDynamic(BookOrder.Trigger.Process,
                () => TryApproveLines() ? BookOrder.State.LinesApproved : BookOrder.State.InsufficientInventory)
            .Permit(BookOrder.Trigger.Cancel, BookOrder.State.Cancelled);

        _machine.Configure(BookOrder.State.LinesApproved)
            .PermitDynamic(BookOrder.Trigger.Process,
                () => TryPay() ? BookOrder.State.PaymentApproved : BookOrder.State.PaymentRejected)
            .Permit(BookOrder.Trigger.Cancel, BookOrder.State.Cancelled);

        _machine.Configure(BookOrder.State.InsufficientInventory)
            .SubstateOf(BookOrder.State.Draft);

        _machine.Configure(BookOrder.State.PaymentApproved)
            .PermitDynamic(BookOrder.Trigger.Process,
                () => TryDeliver() ? BookOrder.State.Delivered : BookOrder.State.Returned)
            .Permit(BookOrder.Trigger.Cancel, BookOrder.State.Cancelled);

        _machine.Configure(BookOrder.State.PaymentRejected)
            .SubstateOf(BookOrder.State.LinesApproved);

        _machine.Configure(BookOrder.State.Returned)
            .SubstateOf(BookOrder.State.PaymentApproved);

        _machine.Configure(BookOrder.State.Cancelled)
            .OnEntry(CancelOrder);
    }

    private void CancelOrder()
    {
        switch (_machine.State)
        {
            case >=BookOrder.State.Delivered:
                break; // Can't cancel delivered order
            case >=BookOrder.State.PaymentApproved:
                // If the status is beyond "Payment approved", need to return the payment
                ReturnPayment();
                // Go to return inventory case
                goto case BookOrder.State.LinesApproved;
            case BookOrder.State.LinesApproved:
            case >=BookOrder.State.LinesApproved:
                // If the status is beyond "Lines approved", need to return the inventory
                ReturnInventory();
                break;
        }
    }
    
    private bool TryApproveLines()
    {
        _order.OrderLines.ForEach(line => _invSvc.AllocateLine(line));
        if (_order.OrderLines.Any(l => l.LineState != OrderLine.State.Allocated))
            return false;
        
        _order.OrderLines.ForEach(line => line.Approve());
        return true;
    }

    private void ReturnInventory()
    {
        _order.OrderLines.ForEach(line => _invSvc.CancelLine(line));
    }
    
    private decimal GetChargeAmount()
    {
        return _order.OrderLines
            .Where(l => l.LineState == OrderLine.State.Approved)
            .Select(l => l.Allocated * l.Price)
            .Sum();
    }
    private bool TryPay()
    {
        return _order.PaymentInfo is not null && _paySvc.TryChargePayment(_order.PaymentInfo, GetChargeAmount());
    }
    
    private void ReturnPayment()
    {
        if (_order.PaymentInfo is null)
            return;
        _paySvc.ReturnPayment(_order.PaymentInfo, GetChargeAmount());
    }


    private bool TryDeliver()
    {
        return _order.DeliveryDestination is not null && _delSvc.TryDelivery(_order.DeliveryDestination!);
    }
}