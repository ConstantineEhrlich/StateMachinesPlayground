using System.Runtime.CompilerServices;
using BookStore.Services.Interfaces;

namespace BookStore.Services.Classes;

public class DeliveryService: IDeliveryService
{
    private static string[] _destinations = new[]
    {
        "Haifa",
        "Tel-Aviv",
        "Jerusalem",
    };

    public DeliveryService()
    {
        
    }


    public async Task<bool> TryDeliveryAsync(string deliveryDestination)
    {
        await Task.Delay(300);
        return _destinations.Contains(deliveryDestination);
    }
}