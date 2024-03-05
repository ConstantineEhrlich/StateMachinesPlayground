using BookStore.Models;
using BookStore.Services.Classes;
using BookStore.Services.Interfaces;
using BookOrderProcessor = BookStore.BookOrderProcessor;

namespace UnitTests.ServicesTests;

[TestClass]
public class OrderProcessorTests
{
    private readonly IInventoryService _inv = new InventoryService(InventoryServiceTests.DummyInventory());
    private readonly IDeliveryService _dlv = new DeliveryService();
    private readonly IPaymentService _pay = new PaymentService();

    private static BookOrder GetOrder()
    {
        BookOrder order = new()
        {
            OrderId = "Order1", 
            DeliveryDestination = "Haifa",
            PaymentInfo = new("Card", 10000),
            OrderLines = new List<OrderLine>()
            {
                new("Red Book", 10, 15),
                new("Green Book", 20, 25),
                new("Blue Book", 30, 35),
                new("Black Book", 40, 45),
            }
        };
        return order;
    }

    
    [TestMethod]
    public void ProcessToLinesApproved()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        
        proc.Process();
        
        Assert.AreEqual(BookOrder.State.LinesApproved, order.OrderStatus);

    }
    
    [TestMethod]
    public void ProcessToInsufficientInventory()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        order.OrderLines[0].UpdateOrdered(200);
        proc.Process();
        
        Assert.AreEqual(BookOrder.State.InsufficientInventory, order.OrderStatus);

    }
    
    [TestMethod]
    public void DoubleProcessToInsufficientInventory()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        order.OrderLines[0].UpdateOrdered(200);
        proc.Process();
        proc.Process(); // Still insufficient inventory
        
        Assert.AreEqual(BookOrder.State.InsufficientInventory, order.OrderStatus);

    }
    
    [TestMethod]
    public void FailToUpdateAfterApprove()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        
        proc.Process();

        Assert.ThrowsException<InvalidOperationException>(() => order.OrderLines[0].UpdateOrdered(100));
    }

    [TestMethod]
    public void ProcessToPaymentApproved()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        
        proc.Process(); // To lines approved
        proc.Process(); // To payment
        Assert.AreEqual(BookOrder.State.PaymentApproved, order.OrderStatus);
    }
    
    [TestMethod]
    public void ProcessToPaymentRejected()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        order.PaymentInfo!.BalanceAmount = 10;
        proc.Process(); // To lines approved
        proc.Process(); // To payment
        Assert.AreEqual(BookOrder.State.PaymentRejected, order.OrderStatus);
    }
    
    [TestMethod]
    public void DoubleProcessToPaymentRejected()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        order.PaymentInfo!.BalanceAmount = 10;
        proc.Process(); // To lines approved
        proc.Process(); // To payment
        proc.Process(); // Still rejected
        Assert.AreEqual(BookOrder.State.PaymentRejected, order.OrderStatus);
    }
    
    [TestMethod]
    public void ProcessToDelivered()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        
        proc.Process(); // To lines approved
        proc.Process(); // To payment
        proc.Process(); // To delivery
        Assert.AreEqual(BookOrder.State.Delivered, order.OrderStatus);
    }
    
    [TestMethod]
    public void ProcessToReturned()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        order.DeliveryDestination = "London";
        proc.Process(); // To lines approved
        proc.Process(); // To payment
        proc.Process(); // To delivery
        Assert.AreEqual(BookOrder.State.Returned, order.OrderStatus);
    }
    
    [TestMethod]
    public void DoubleProcessToReturned()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        order.DeliveryDestination = "London";
        proc.Process(); // To lines approved
        proc.Process(); // To payment
        proc.Process(); // To delivery
        proc.Process(); // Still returned
        Assert.AreEqual(BookOrder.State.Returned, order.OrderStatus);
    }

    
    [TestMethod]
    public void CancelFromDraft()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        proc.Cancel();
        Assert.AreEqual(BookOrder.State.Cancelled, order.OrderStatus);
    }
    
    
    [TestMethod]
    public void CancelFromInsufficientInventory()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        order.OrderLines[0].UpdateOrdered(200);
        proc.Process();
        proc.Cancel();
        Assert.AreEqual(BookOrder.State.Cancelled, order.OrderStatus);
    }
    
    
    [TestMethod]
    public void CancelFromLinesApproved()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        order.OrderLines[0].UpdateOrdered(200);
        proc.Process();
        proc.Cancel();
        Assert.AreEqual(BookOrder.State.Cancelled, order.OrderStatus);
    }
    
    
    
    [TestMethod]
    public void CancelFromPaymentRejected()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        order.PaymentInfo!.BalanceAmount = 10;
        proc.Process(); // To lines approved
        proc.Process(); // To payment
        proc.Cancel();
        Assert.AreEqual(BookOrder.State.Cancelled, order.OrderStatus);
    }


    [TestMethod]
    public void CancelFromPaymentApproved()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        proc.Process(); // To lines approved
        proc.Process(); // To payment
        proc.Cancel();
        Assert.AreEqual(BookOrder.State.Cancelled, order.OrderStatus);
    }



    [TestMethod]
    public void CancelFromReturned()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        order.DeliveryDestination = "London";
        proc.Process(); // To lines approved
        proc.Process(); // To payment
        proc.Process(); // To delivery
        proc.Cancel();
        Assert.AreEqual(BookOrder.State.Cancelled, order.OrderStatus);
    }



    [TestMethod]
    public void CancelFailFromDelivered()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        proc.Process(); // To lines approved
        proc.Process(); // To payment
        proc.Process(); // To delivery
        Assert.ThrowsException<InvalidOperationException>(() => proc.Cancel());
    }



    [TestMethod]
    public void CancelFailFromCanceled()
    {
        var order = GetOrder();
        var proc = new BookOrderProcessor(order, _dlv, _pay, _inv);
        proc.Cancel(); // First time
        Assert.ThrowsException<InvalidOperationException>(() => proc.Cancel());
    }

    
    
}