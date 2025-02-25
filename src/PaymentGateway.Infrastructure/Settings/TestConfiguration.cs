using Microsoft.Extensions.Configuration;

namespace PaymentGateway.Infrastructure.Settings;

public static class TestConfiguration
{
    private static readonly AsyncLocal<Action<IConfigurationBuilder>> _current = new();

    public static IConfigurationBuilder AddTestConfiguration(this IConfigurationBuilder configurationBuilder)
    {
        if (_current.Value is { } configure)
        {
            configure(configurationBuilder);
        }

        return configurationBuilder;
    }

    public static void Create(Action<IConfigurationBuilder> configure)
    {
        _current.Value = configure;
    }
}