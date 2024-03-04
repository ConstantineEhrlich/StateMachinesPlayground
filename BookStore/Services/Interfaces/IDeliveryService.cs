namespace BookStore.Services.Interfaces;

public interface IDeliveryService
{
    /// <summary>
    /// Attempts delivery to the specified destination
    /// </summary>
    /// <param name="deliveryDestination">Delivery destination name</param>
    /// <returns>True for successful delivery, false for fail</returns>
    bool TryDelivery(string deliveryDestination);
}