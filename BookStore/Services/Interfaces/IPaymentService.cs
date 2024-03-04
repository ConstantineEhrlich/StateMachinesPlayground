using BookStore.Models;

namespace BookStore.Services.Interfaces;

public interface IPaymentService
{
    /// <summary>
    /// Attempts to charge payment from the provided payment term
    /// </summary>
    /// <param name="paymentInfo">The payment term</param>
    /// <param name="amount">Amount to charge</param>
    /// <returns>True, if the operation succeeds, false for fail</returns>
    Task<bool> TryChargePaymentAsync(PaymentInfo paymentInfo, decimal amount);
}