namespace BookStore;

public interface IBookOrderProcessor
{
    void Process();
    void Cancel();
}