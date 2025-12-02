namespace EmailSender.Infrastructure.Configuration;

public class KafkaConfiguration
{
    public string Server { get; set; }
    public string CoinbaseOhlcTopicName { get; set; }
}
