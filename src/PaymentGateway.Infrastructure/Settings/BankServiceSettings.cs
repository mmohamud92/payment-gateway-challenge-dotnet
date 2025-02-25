namespace PaymentGateway.Infrastructure.Settings;

public class BankServiceSettings
{
    public string ConfigurationSectionKey { get; set; } = nameof(BankServiceSettings);
    public string BaseUrl { get; set; } = string.Empty;
}