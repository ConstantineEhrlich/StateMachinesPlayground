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

    }
    
    private bool TryApproveLines()
    {
        _order.OrderLines.ForEach(line => _invSvc.AllocateLine(line));
        if (_order.OrderLines.Any(l => l.LineState != OrderLine.State.Allocated))
            return false;
        
        _order.OrderLines.ForEach(line => line.Approve());
        return true;
    }

    private bool TryPay()
    {
        decimal chargeAmount = _order.OrderLines.Select(l => l.Allocated * l.Price).Sum();
        return _order.PaymentInfo is not null && _paySvc.TryChargePayment(_order.PaymentInfo, chargeAmount);
    }


    private bool TryDeliver()
    {
        return _order.DeliveryDestination is not null && _delSvc.TryDelivery(_order.DeliveryDestination!);
    }
}