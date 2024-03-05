using BookStore.Services.Classes;

namespace UnitTests.ServicesTests;

[TestClass]
public class DeliveryServiceTests
{
    [TestMethod]
    public void TestSuccessDelivery()
    {
        DeliveryService svc = new();
        Assert.IsTrue(svc.TryDelivery("Haifa"));
    }
    
    [TestMethod]
    public void TestFailDelivery()
    {
        DeliveryService svc = new();
        Assert.IsFalse(svc.TryDelivery("London"));
    }
}