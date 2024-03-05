using BookStore.Models;
using BookStore.Services.Interfaces;

namespace BookStore;

public class OrderProcessorFactory
{
    private readonly IDeliveryService _delSvc;
    private readonly IPaymentService _paySvc;
    private readonly IInventoryService _invSvc;

    public OrderProcessorFactory(IDeliveryService delSvc, IPaymentService paySvc, IInventoryService invSvc)
    {
        _delSvc = delSvc;
        _paySvc = paySvc;
        _invSvc = invSvc;
    }

    public IBookOrderProcessor GetProcessor(BookOrder order)
    {
        return new BookOrderProcessor(order, _delSvc, _paySvc, _invSvc);
    }
}