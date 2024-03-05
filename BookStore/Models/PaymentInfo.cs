namespace BookStore.Models;

public class PaymentInfo
{
    public string CardId { get; init; }
    public decimal BalanceAmount { get; set; }

    public PaymentInfo(string cardId, decimal balanceAmount)
    {
        CardId = cardId;
        BalanceAmount = balanceAmount;
    }
    
}