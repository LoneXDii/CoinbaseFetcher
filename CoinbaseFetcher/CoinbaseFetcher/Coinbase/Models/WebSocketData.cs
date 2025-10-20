using System.Text.Json.Serialization;

namespace CoinbaseFetcher.Coinbase.Models;

public class WebSocketData
{
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; }
    
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    
    [JsonPropertyName("time")]
    public DateTime Time { get; set; }
}