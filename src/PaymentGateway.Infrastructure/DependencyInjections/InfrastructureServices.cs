using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using PaymentGateway.Application.Interfaces;
using PaymentGateway.Domain.Interfaces;
using PaymentGateway.Infrastructure.Authentication;
using PaymentGateway.Infrastructure.Persistence;
using PaymentGateway.Infrastructure.Services;
using PaymentGateway.Infrastructure.Settings;

namespace PaymentGateway.Infrastructure.DependencyInjections;

public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        string sectionName = new BankServiceSettings().ConfigurationSectionKey;
        services.Configure<BankServiceSettings>(options =>
        {
            configuration.GetSection(sectionName).Bind(options);
        });

        services.AddMemoryCache();

        services.AddSingleton<IPaymentRepository, PaymentRepository>();

        services.AddHttpClient<IBankService, BankService>((serviceProvider, client) =>
        {
            BankServiceSettings options = serviceProvider.GetRequiredService<IOptions<BankServiceSettings>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        services.AddJwtAuthentication(configuration);
        services.AddCustomAuthorisationPolicies();
        services.AddOpenIddictInfrastructure();

        return services;
    }
}