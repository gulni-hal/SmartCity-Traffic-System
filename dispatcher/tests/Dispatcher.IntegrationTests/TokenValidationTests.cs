using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Headers;
using Dispatcher.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dispatcher.IntegrationTests;

public class TokenValidationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TokenValidationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Traffic_With_Invalid_Token_Should_Return_401()
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IAuthValidationService>();
                services.AddSingleton<IAuthValidationService>(
                    new FakeAuthValidationService(new AuthValidationResult
                    {
                        IsValid = false
                    }));
            });
        });

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "invalid-token");

        var response = await client.GetAsync("/api/traffic/live");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private sealed class FakeAuthValidationService : IAuthValidationService
    {
        private readonly AuthValidationResult _result;

        public FakeAuthValidationService(AuthValidationResult result)
        {
            _result = result;
        }

        public Task<AuthValidationResult> ValidateAsync(string token)
        {
            return Task.FromResult(_result);
        }
    }
}
