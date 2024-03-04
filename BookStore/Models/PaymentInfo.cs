namespace BookStore.Models;

public class PaymentInfo
{
    public string CardId { get; init; }
    public decimal BalanceAmount { get; set; }

    public PaymentInfo(string id, decimal balance)
    {
        CardId = id;
        BalanceAmount = balance;
    }
    
}