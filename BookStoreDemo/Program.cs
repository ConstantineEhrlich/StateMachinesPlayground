using System.Collections.Concurrent;
using BookStore;
using BookStore.Models;
using BookStore.Services.Classes;
using BookStore.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BookStoreDemo;

class Program
{
    public static void Main()
    {
        Console.WriteLine("\t*** Initial inventory: ***");
        var serviceProvider = GetServiceProvider();
        var inventory = (InventoryService)serviceProvider.GetService<IInventoryService>()!;
        inventory.PrintInventory();
        
        Console.WriteLine("\n\t*** Create new order ***");
        var order = DemoOrder();
        order.PrintOrder();
        
        Console.WriteLine("\n\t*** Process the order => Insufficient Inventory ***");
        var procFactory = serviceProvider.GetService<OrderProcessorFactory>();
        var proc = procFactory!.GetProcessor(order);
        proc.Process();
        order.PrintOrder();

        Console.WriteLine("\n\t*** Update lines 0 and 2  ***");
        order.OrderLines[0].UpdateOrdered(25);
        order.OrderLines[2].UpdateOrdered(50);
        order.PrintOrder();

        Console.WriteLine("\n\t*** Process the order => Lines Approved ***");
        proc.Process();
        order.PrintOrder();
        
        Console.WriteLine("\n\t*** Inventory at this stage: ***");
        inventory.PrintInventory();
        
        Console.WriteLine("\n\t*** Process the order => Payment Rejected ***");
        proc.Process();
        order.PrintOrder();

        Console.WriteLine("\n\t*** Update payment info ***");
        order.PaymentInfo = new("Master Card", 10000);
        order.PrintOrder();
        
        Console.WriteLine("\n\t*** Process the order => Payment Approved ***");
        proc.Process();
        order.PrintOrder();
        
        Console.WriteLine("\n\t*** Process the order => Delivery Failed ***");
        proc.Process();
        order.PrintOrder();

        Console.WriteLine("\n\t*** Cancel the order => Refund a payment ***");
        proc.Cancel();
        order.PrintOrder();
        
        Console.WriteLine("\n\t*** Cancel the order => Inventory returns ***");
        inventory.PrintInventory();
    }


    private static IServiceProvider GetServiceProvider()
    {
        ServiceCollection services = new();
        services.AddSingleton<IInventoryService>(new InventoryService(DemoInventory()));
        services.AddSingleton<IDeliveryService, DeliveryService>();
        services.AddSingleton<IPaymentService, PaymentService>();

        services.AddScoped<OrderProcessorFactory>();

        return services.BuildServiceProvider();
    }
    
    private static ConcurrentDictionary<string, int> DemoInventory()
    {
        ConcurrentDictionary<string, int> i = new();
        i.TryAdd("Red Book", 100);
        i.TryAdd("Green Book", 100);
        i.TryAdd("Blue Book", 100);
        i.TryAdd("Black Book", 100);
        i.TryAdd("White Book", 100);
        return i;
    }

    private static BookOrder DemoOrder()
    {
        return new()
        {
            OrderId = "ORD_001", 
            DeliveryDestination = "London",
            PaymentInfo = new("Visa", 100),
            OrderLines = new List<OrderLine>()
            {
                new("Red Book", 250, 15),
                new("Green Book", 20, 25),
                new("Blue Book", 30, 35),
                new("Black Book", 40, 45),
            }
        };
    }
}