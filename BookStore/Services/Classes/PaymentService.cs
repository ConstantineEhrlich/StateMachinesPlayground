using BookStore.Models;
using BookStore.Services.Interfaces;

namespace BookStore.Services.Classes;

public class PaymentService: IPaymentService
{
    public bool TryChargePayment(PaymentInfo paymentInfo, decimal chargeAmount)
    {
        if (paymentInfo.BalanceAmount < chargeAmount) return false;
        paymentInfo.BalanceAmount -= chargeAmount;
        return true;
    }
}