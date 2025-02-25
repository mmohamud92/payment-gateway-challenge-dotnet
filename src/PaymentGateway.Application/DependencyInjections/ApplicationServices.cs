using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Application.Handlers;
using PaymentGateway.Application.MappingProfiles;

namespace PaymentGateway.Application.DependencyInjections;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetPaymentByIdHandler).Assembly));
        services.AddAutoMapper(typeof(PaymentMappingProfile).Assembly);

        return services;
    }
}