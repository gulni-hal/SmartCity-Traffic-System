using System.Net;
using System.Net.Http.Headers;
using Dispatcher.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dispatcher.IntegrationTests;

public class DownstreamErrorTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DownstreamErrorTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Traffic_With_Valid_Token_And_Unavailable_Service_Should_Return_502()
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(IAuthValidationService));
                services.RemoveAll(typeof(IAuditLogRepository));

                services.AddSingleton<IAuthValidationService>(
                    new FakeAuthValidationService(new AuthValidationResult
                    {
                        IsValid = true,
                        Username = "ali",
                        Role = "TrafficPolice"
                    }));

                services.AddSingleton<IAuditLogRepository>(new FakeAuditLogRepository());
            });
        });

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "valid-token");

        var response = await client.GetAsync("/api/traffic/live");

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
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

    private sealed class FakeAuditLogRepository : IAuditLogRepository
    {
        public Task CreateAsync(RequestAuditLog log)
        {
            return Task.CompletedTask;
        }
    }
}
