using System.Globalization;
using Stateless;

namespace BookStore.Models;

public class BookOrder
{
    internal StateMachine<BookOrder.State, BookOrder.Trigger>? Machine;
    internal enum Trigger { Process, Cancel }
    public enum State
    {
        Draft = 0,
        LinesApproved = 20,
        InsufficientInventory = 40,
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

    public void PrintOrder()
    {
        Console.WriteLine($"Order:\t\t\t{OrderId}");
        Console.WriteLine($"Status:\t\t\t{OrderStatus}");
        Console.WriteLine($"Destination:\t{DeliveryDestination}");
        Console.WriteLine($"Card Id:\t\t{PaymentInfo?.CardId ?? "No card"}");
        Console.WriteLine($"Balance:\t\t{PaymentInfo?.BalanceAmount}");
        PrintOrderLines();
        Console.WriteLine("\n\n\n");
    }

    private void PrintOrderLines()
    {
        if (!OrderLines.Any())
        {
            Console.WriteLine("*** Order is empty ***");
            return;
        }

        string hdrBook = "Book Name";
        int maxBookLen = 1 + Math.Max(OrderLines.Max(l => l.BookId?.Length) ?? 0, hdrBook.Length);
        string hdrQty = "Ordered";
        int maxQtyLen = 1 + Math.Max(OrderLines.Max(l => l.Ordered.ToString().Length), hdrQty.Length);
        string hdrAlc = "Allocated";
        int maxAlcLen = 1 + Math.Max(OrderLines.Max(l => l.Allocated.ToString().Length), hdrAlc.Length);
        string hdrPrc = "Price";
        int maxPrcLen = 1 + Math.Max(OrderLines.Max(l => l.Price.ToString(CultureInfo.InvariantCulture).Length), hdrPrc.Length);

        string format = $"| {{0,-{maxBookLen}}} | {{1, {maxQtyLen}}} | {{2, {maxAlcLen}}} | {{3, {maxPrcLen}}} |";

        Console.WriteLine(format, hdrBook, hdrQty, hdrAlc, hdrPrc);
        Console.WriteLine(new string('-', maxBookLen + maxQtyLen + maxAlcLen + maxPrcLen + 13));

        foreach (OrderLine line in OrderLines)
        {
            Console.WriteLine(format, line.BookId, line.Ordered, line.Allocated, line.Price);
        }
    }
    
}



