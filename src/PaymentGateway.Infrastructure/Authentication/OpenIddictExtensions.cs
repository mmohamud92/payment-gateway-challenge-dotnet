using System.Security.Claims;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using OpenIddict.Abstractions;
using OpenIddict.Server;

using static OpenIddict.Abstractions.OpenIddictConstants;

namespace PaymentGateway.Infrastructure.Authentication;

public static class OpenIddictExtensions
{
    public static IServiceCollection AddOpenIddictInfrastructure(this IServiceCollection services)
    {
        // Register an EF Core DbContext using the in-memory provider.
        services.AddDbContext<DbContext>(options =>
        {
            // Use an in-memory database for demo purposes.
            options.UseInMemoryDatabase("OpenIddictDemoDb");
            options.UseOpenIddict();
        });

        services.AddOpenIddict()
            // Register the core services with EF Core stores.
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<DbContext>();
            })
            .AddServer(options =>
            {
                // Set the token endpoint URI.
                options.SetTokenEndpointUris("/connect/token");

                // Enable the resource owner password flow.
                options.AllowPasswordFlow();

                options.DisableAccessTokenEncryption();

                // Accept anonymous clients (no client secret required).
                options.AcceptAnonymousClients();

                // Use development encryption and signing credentials.
                options.AddDevelopmentSigningCertificate();
                options.AddDevelopmentEncryptionCertificate();

                // Integrate OpenIddict with ASP.NET Core.
                options.UseAspNetCore()
                    .DisableTransportSecurityRequirement();

                options.RegisterScopes(Constants.ApiScopes.PaymentReadScope, Constants.ApiScopes.PaymentWriteScope);

                // Add an event handler for handling password grant token requests.
                options.AddEventHandler<OpenIddictServerEvents.HandleTokenRequestContext>(builder =>
                {
                    builder.UseInlineHandler(context =>
                    {
                        // Only process password grant requests.
                        if (!context.Request.IsPasswordGrantType())
                        {
                            return default;
                        }

                        // Validate the resource owner credentials.
                        if (context.Request.Username != "superuser" || context.Request.Password != "superpassword")
                        {
                            context.Reject(
                                Errors.InvalidGrant,
                                "Invalid username or password.");
                            return default;
                        }

                        // Create a ClaimsIdentity using OpenIddict's default authentication scheme.
                        ClaimsIdentity identity = new(OpenIddict.Server.AspNetCore.OpenIddictServerAspNetCoreDefaults
                            .AuthenticationScheme);

                        // Create and add the subject claim.
                        Claim subjectClaim = new(Claims.Subject, "superuser");
                        subjectClaim.SetDestinations(new[] { Destinations.AccessToken, Destinations.IdentityToken });
                        identity.AddClaim(subjectClaim);

                        string merchantId = Guid.NewGuid().ToString();
                        Claim merchantClaim = new(Constants.ClaimsExtension.MerchantId, merchantId);
                        merchantClaim.SetDestinations(Destinations.AccessToken, Destinations.IdentityToken);
                        identity.AddClaim(merchantClaim);

                        // Create and add the read scope claim.
                        Claim readScopeClaim = new("scope", Constants.ApiScopes.PaymentReadScope);
                        readScopeClaim.SetDestinations(Destinations.AccessToken, Destinations.IdentityToken);
                        identity.AddClaim(readScopeClaim);

                        // Create and add the write scope claim.
                        Claim writeScopeClaim = new("scope", Constants.ApiScopes.PaymentWriteScope);
                        writeScopeClaim.SetDestinations(Destinations.AccessToken, Destinations.IdentityToken);
                        identity.AddClaim(writeScopeClaim);

                        // Build a ClaimsPrincipal from the identity.
                        ClaimsPrincipal principal = new(identity);
                        principal.SetScopes(Constants.ApiScopes.PaymentReadScope, Constants.ApiScopes.PaymentWriteScope);

                        // Mark the token request as valid.
                        context.SignIn(principal);

                        return default;
                    });
                });
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        return services;
    }
}