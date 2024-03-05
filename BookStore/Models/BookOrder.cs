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
        Console.WriteLine($"Order:\t\t{OrderId}");
        Console.WriteLine($"Status:\t\t{OrderStatus}");
        Console.WriteLine($"Destination:\t{DeliveryDestination}");
        Console.WriteLine($"Card Id:\t{PaymentInfo?.CardId ?? "No card"}");
        Console.WriteLine($"Balance:\t{PaymentInfo?.BalanceAmount}");
        PrintOrderLines();
        Console.WriteLine("\n");
    }

    private void PrintOrderLines()
    {
        if (!OrderLines.Any())
        {
            Console.WriteLine("*** Order is empty ***");
            return;
        }

        string hdrNum = "##";
        int maxNumLen = 1 + Math.Min(OrderLines.Count.ToString().Length, hdrNum.Length);
        string hdrBook = "Book Name";
        int maxBookLen = 1 + Math.Max(OrderLines.Max(l => l.BookId?.Length) ?? 0, hdrBook.Length);
        string hdrQty = "Ordered";
        int maxQtyLen = 1 + Math.Max(OrderLines.Max(l => l.Ordered.ToString().Length), hdrQty.Length);
        string hdrAlc = "Allocated";
        int maxAlcLen = 1 + Math.Max(OrderLines.Max(l => l.Allocated.ToString().Length), hdrAlc.Length);
        string hdrPrc = "Price";
        int maxPrcLen = 1 + Math.Max(OrderLines.Max(l => l.Price.ToString(CultureInfo.InvariantCulture).Length), hdrPrc.Length);
        string hdrStat = "Status";
        int maxStatLen = 1 + Math.Max(OrderLines.Max(l => l.LineState.ToString().Length), hdrStat.Length);

        string format = $"| {{0,-{maxNumLen}}} | {{1,-{maxBookLen}}} | {{2, {maxQtyLen}}} | {{3, {maxAlcLen}}} | {{4, {maxPrcLen}}} | {{5, {maxStatLen}}} |";

        Console.WriteLine(format, hdrNum, hdrBook, hdrQty, hdrAlc, hdrPrc, hdrStat);
        Console.WriteLine(new string('-', maxNumLen + maxBookLen + maxQtyLen + maxAlcLen + maxPrcLen + maxStatLen + 19));

        for (var i = 0; i < OrderLines.Count; i++)
        {
            OrderLine line = OrderLines[i];
            Console.WriteLine(format, i, line.BookId, line.Ordered, line.Allocated, line.Price, line.LineState);
        }
    }
    
}



