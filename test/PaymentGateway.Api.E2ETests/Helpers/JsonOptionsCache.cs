using System.Text.Json;

namespace PaymentGateway.Api.E2ETests.Helpers;

public static class JsonOptionsCache
{
    public static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };
}