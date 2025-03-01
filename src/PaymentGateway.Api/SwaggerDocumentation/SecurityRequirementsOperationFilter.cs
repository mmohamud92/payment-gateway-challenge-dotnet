using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace PaymentGateway.Api.SwaggerDocumentation;

public class SecurityRequirementsOperationFilter(IOptions<AuthorizationOptions> authOptions) : IOperationFilter
{
    private readonly AuthorizationOptions? _authOptions =
        authOptions?.Value ?? throw new ArgumentNullException(nameof(authOptions));

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        IEnumerable<string?> requiredPolicies = context.ApiDescription.ActionDescriptor.EndpointMetadata
            .OfType<AuthorizeAttribute>()
            .Select(attr => attr.Policy)
            .Where(policy => !string.IsNullOrWhiteSpace(policy))
            .Distinct();

        List<string> requiredScopes = requiredPolicies
            .Select(policyName => _authOptions?.GetPolicy(policyName!))
            .Where(policy => policy != null)
            .SelectMany(policy => policy!.Requirements)
            .OfType<ClaimsAuthorizationRequirement>()
            .Where(req => req.ClaimType == "scope")
            .SelectMany(req => req.AllowedValues!)
            .Distinct()
            .ToList();

        if (requiredScopes.Count == 0)
        {
            return;
        }

        // Add responses for 401 and 403.
        operation.Responses.Add(StatusCodes.Status401Unauthorized.ToString(),
            new OpenApiResponse { Description = "Unauthorised" });
        operation.Responses.Add(StatusCodes.Status403Forbidden.ToString(),
            new OpenApiResponse { Description = "Forbidden" });

        OpenApiSecurityScheme oAuthScheme = new()
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "OAuth2" }
        };

        operation.Security = new List<OpenApiSecurityRequirement> { new() { [oAuthScheme] = requiredScopes } };
    }
}