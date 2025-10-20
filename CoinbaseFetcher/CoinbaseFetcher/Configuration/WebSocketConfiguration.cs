namespace CoinbaseFetcher.Configuration;

public class WebSocketConfiguration
{
    public string Url { get; set; }
    public List<string> Channels { get; set; }
    public List<string> Products { get; set; }
}