using Microsoft.OpenApi.Models;

namespace PaymentGateway.Api.SwaggerDocumentation;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "PaymentGateway API", Version = "1.0" });
            opt.AddSecurityDefinition("OAuth2",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OpenIdConnect,
                    OpenIdConnectUrl = new Uri("/.well-known/openid-configuration", UriKind.Relative)
                });
            // opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
            //     $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            opt.OperationFilter<SecurityRequirementsOperationFilter>();
            opt.SupportNonNullableReferenceTypes();
        });

        return services;
    }
}