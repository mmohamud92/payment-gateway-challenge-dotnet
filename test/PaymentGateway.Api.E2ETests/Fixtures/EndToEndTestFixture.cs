using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using PaymentGateway.Api.E2ETests.Helpers;
using PaymentGateway.Infrastructure.Settings;

namespace PaymentGateway.Api.E2ETests.Fixtures;

public class EndToEndTestFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private ExternalServicesFixture? DockerFixture { get; set; }
    public HttpClient? Client { get; private set; }
    private const string Url = "https://localhost:5067/";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseUrls(Url);

        builder.ConfigureTestServices(services =>
        {
            // Override the JWT Bearer options for tests.
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                JwtSecurityTokenHandler tokenHandler = new();

                options.RequireHttpsMetadata = false;
                options.TokenHandlers.Clear();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = false,
                    SignatureValidator = (token, parameters) => new JwtSecurityToken(token)
                };

                options.TokenHandlers.Add(tokenHandler);

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        Claim? scopeClaim = context.Principal?.FindFirst("scope");
                        if (scopeClaim != null && scopeClaim.Value.Contains(' '))
                        {
                            string[] scopes = scopeClaim.Value.Split(' ');
                            ClaimsIdentity identity = (ClaimsIdentity)context.Principal?.Identity!;
                            identity.RemoveClaim(scopeClaim);
                            foreach (string scope in scopes)
                            {
                                identity.AddClaim(new Claim("scope", scope));
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        });

        TestConfiguration.Create(configBuilder =>
        {
            configBuilder.AddInMemoryCollection(new List<KeyValuePair<string, string?>>
            {
                new("BankServiceSettings:BaseUrl", "http://localhost:8080/"), new("AuthSettings:ValidIssuer", Url)
            });
        });
    }

    public async Task InitializeAsync()
    {
        DockerFixture = new ExternalServicesFixture();
        await DockerFixture.InitializeAsync();

        Client = CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri(Url) });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        Client?.Dispose();
        await DisposeAsync();
        if (DockerFixture != null)
        {
            await DockerFixture.DisposeAsync();
        }
    }
}