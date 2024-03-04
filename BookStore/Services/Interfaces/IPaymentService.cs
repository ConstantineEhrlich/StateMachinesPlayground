using BookStore.Models;

namespace BookStore.Services.Interfaces;

public interface IPaymentService
{
    /// <summary>
    /// Attempts to charge payment from the provided payment term
    /// </summary>
    /// <param name="paymentInfo">The payment term</param>
    /// <param name="chargeAmount">Amount to charge</param>
    /// <returns>True, if the operation succeeds, false for fail</returns>
    bool TryChargePayment(PaymentInfo paymentInfo, decimal chargeAmount);

    /// <summary>
    /// Returns a specified amount to the payment term.
    /// </summary>
    /// <param name="paymentInfo">The payment term</param>
    /// <param name="returnAmount">Amount to return</param>
    void ReturnPayment(PaymentInfo paymentInfo, decimal returnAmount);
}