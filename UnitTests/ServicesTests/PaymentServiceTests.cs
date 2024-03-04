using BookStore.Models;
using BookStore.Services.Classes;

namespace UnitTests.ServicesTests;

[TestClass]
public class PaymentServiceTests
{
    [TestMethod]
    public void SuccessfulPaymentReturnsTrueSameAmount()
    {
        PaymentService ps = new();
        PaymentInfo pi = new("Card", 100);
        Assert.IsTrue(ps.TryChargePayment(pi, 100));
    }

    [TestMethod] public void SuccessfulPaymentReturnsTrueSmallerAmount()
    {
        PaymentService ps = new();
        PaymentInfo pi = new("Card", 100);
        Assert.IsTrue(ps.TryChargePayment(pi, 80));
    }
    
    [TestMethod]
    public void FailedPaymentReturnsFalse()
    {
        PaymentService ps = new();
        PaymentInfo pi = new("Card", 100);
        Assert.IsFalse(ps.TryChargePayment(pi, 200));
    }
    
    [TestMethod]
    public void SuccessfulPaymentUpdatesAmount()
    {
        PaymentService ps = new();
        PaymentInfo pi = new("Card", 100);
        ps.TryChargePayment(pi, 20);
        Assert.AreEqual(80, pi.BalanceAmount);
    }

    [TestMethod]
    public void FailedPaymentDoesNotUpdateAmount()
    {
        PaymentService ps = new();
        PaymentInfo pi = new("Card", 100);
        ps.TryChargePayment(pi, 120);
        Assert.AreEqual(100, pi.BalanceAmount);
    }
    
    [TestMethod]
    public void ReturnUpdatesAmount()
    {
        PaymentService ps = new();
        PaymentInfo pi = new("Card", 100);
        ps.ReturnPayment(pi, 20);
        Assert.AreEqual(120, pi.BalanceAmount);
    }
    
}