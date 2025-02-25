namespace PaymentGateway.Infrastructure.Settings;

public class AuthSettings
{
    public string ConfigurationSectionKey { get; set; } = nameof(AuthSettings);
    public string ValidIssuer { get; set; } = string.Empty;
}