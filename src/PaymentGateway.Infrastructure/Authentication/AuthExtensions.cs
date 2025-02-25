using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using PaymentGateway.Infrastructure.Settings;

namespace PaymentGateway.Infrastructure.Authentication;

public static class AuthExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        AuthSettings authSettings = new();
        string sectionKey = new AuthSettings().ConfigurationSectionKey;
        configuration.GetSection(sectionKey).Bind(authSettings);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters { ValidateAudience = false };
                options.Authority = authSettings.ValidIssuer;
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        Claim? scopeClaim = context.Principal?.FindFirst("scope");
                        if (scopeClaim == null || !scopeClaim.Value.Contains(' '))
                        {
                            return Task.CompletedTask;
                        }

                        string[] scopes = scopeClaim.Value.Split(' ');
                        ClaimsIdentity identity = (ClaimsIdentity)context.Principal?.Identity!;
                        identity.RemoveClaim(scopeClaim);
                        foreach (string scope in scopes)
                        {
                            identity.AddClaim(new Claim("scope", scope));
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        return services;
    }

    public static IServiceCollection AddCustomAuthorisationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(Constants.Policies.PaymentReadPolicy, policy =>
            {
                policy.RequireClaim("scope", Constants.ApiScopes.PaymentReadScope);
            })
            .AddPolicy(Constants.Policies.PaymentWritePolicy, policy =>
            {
                policy.RequireClaim("scope", Constants.ApiScopes.PaymentWriteScope);
            });
        return services;
    }
}