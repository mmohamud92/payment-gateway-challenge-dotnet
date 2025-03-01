using Microsoft.Extensions.Configuration;

namespace PaymentGateway.Infrastructure.Settings;

/// <summary>
/// Provides a way to inject test‐specific configuration into the configuration builder,
/// working around an issue in ASP.NET Core (see: https://github.com/dotnet/aspnetcore/issues/37680).
/// 
/// This class utilises an <see cref="AsyncLocal{T}"/> to store a configuration delegate that can be applied
/// during test configuration. The AsyncLocal ensures that the test configuration action is available only 
/// within the current asynchronous context (i.e., within the scope of a test), thereby preventing any leakage 
/// between tests.
/// 
/// Usage:
/// 
/// 1. In your test setup, call <see cref="Create(Action{IConfigurationBuilder})"/> to set up a configuration delegate.
/// 
/// 2. When building your <see cref="IConfiguration"/>, call the extension method <see cref="AddTestConfiguration"/>
///    on your <see cref="IConfigurationBuilder"/>. If a test configuration delegate is present, it will be invoked,
///    applying any test‐specific configuration settings.
/// </summary>
public static class TestConfiguration
{
    // AsyncLocal is used to store a configuration action that is local to the current async flow.
    private static readonly AsyncLocal<Action<IConfigurationBuilder>> Current = new();

    /// <summary>
    /// Extends the <see cref="IConfigurationBuilder"/> to apply test‐specific configuration.
    /// If a configuration delegate was set via <see cref="Create"/>, it will be invoked on the builder.
    /// </summary>
    /// <param name="configurationBuilder">The configuration builder to customise.</param>
    /// <returns>The same configuration builder with any test‐specific configuration applied.</returns>
    public static IConfigurationBuilder AddTestConfiguration(this IConfigurationBuilder configurationBuilder)
    {
        // If a configuration action exists in the current async context, apply it.
        if (Current.Value is { } configure)
        {
            configure(configurationBuilder);
        }

        return configurationBuilder;
    }

    /// <summary>
    /// Sets the test‐specific configuration action for the current async context.
    /// This allows tests to inject custom configuration settings into the configuration builder.
    /// 
    /// This is a workaround for an open ASP.NET Core issue with configuration in tests:
    /// https://github.com/dotnet/aspnetcore/issues/37680
    /// </summary>
    /// <param name="configure">An action that receives an <see cref="IConfigurationBuilder"/> to configure.</param>
    public static void Create(Action<IConfigurationBuilder> configure)
    {
        Current.Value = configure;
    }
}