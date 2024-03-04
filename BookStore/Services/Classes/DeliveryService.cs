using System.Runtime.CompilerServices;
using BookStore.Services.Interfaces;

namespace BookStore.Services.Classes;

public class DeliveryService: IDeliveryService
{
    private readonly string[] _destinations = new[]
    {
        "Haifa",
        "Tel-Aviv",
        "Jerusalem",
    };

    public bool TryDelivery(string deliveryDestination)
    {
        return _destinations.Contains(deliveryDestination);
    }
}