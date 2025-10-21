namespace CoinbaseFetcher.Configuration;

public class SmtpConfiguration
{
    public string FromEmail { get; set; }
    
    public string ToEmail { get; set; }

    public string Subject { get; set; }

    public string Host { get; set; }

    public int Port { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }
}