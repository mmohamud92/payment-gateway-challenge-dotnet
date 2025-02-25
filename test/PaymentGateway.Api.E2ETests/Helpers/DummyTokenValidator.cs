using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

namespace PaymentGateway.Api.E2ETests.Helpers;

public class DummyTokenValidator : ISecurityTokenValidator
{
    public bool CanValidateToken => true;
    public int MaximumTokenSizeInBytes { get; set; } = int.MaxValue;

    public bool CanReadToken(string securityToken)
    {
        return true;
    }

    public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters,
        out SecurityToken validatedToken)
    {
        // Simply parse the token into a JwtSecurityToken without performing any signature or issuer checks.
        JwtSecurityToken jwt = new(securityToken);
        validatedToken = jwt;
        ClaimsIdentity identity = new(jwt.Claims, "Test");
        return new ClaimsPrincipal(identity);
    }
}