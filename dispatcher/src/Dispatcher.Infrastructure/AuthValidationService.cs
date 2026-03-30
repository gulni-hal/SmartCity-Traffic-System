using System.Net.Http;
using System.Net.Http.Json;
using Dispatcher.Application;

namespace Dispatcher.Infrastructure;

public class AuthValidationService : IAuthValidationService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthValidationService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<AuthValidationResult> ValidateAsync(string token)
    {
        var client = _httpClientFactory.CreateClient("AuthService");

        var response = await client.PostAsJsonAsync("/api/auth/validate", new
        {
            Token = token
        });

        if (!response.IsSuccessStatusCode)
        {
            return new AuthValidationResult { IsValid = false };
        }

        var result = await response.Content.ReadFromJsonAsync<AuthValidationResult>();
        return result ?? new AuthValidationResult { IsValid = false };
    }
}
