namespace CoinbaseFetcher.Domain.Models;

public class TickData
{
    public string ProductId { get; set; }
    public decimal Price { get; set; }
    public DateTime DateTime { get; set; }
}
