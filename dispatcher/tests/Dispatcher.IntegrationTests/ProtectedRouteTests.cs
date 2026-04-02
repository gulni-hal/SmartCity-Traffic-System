using System.Net;
using Dispatcher.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dispatcher.IntegrationTests;

public class ProtectedRouteTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProtectedRouteTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Traffic_Without_Token_Should_Return_401()
    {
        var factory = CreateFactoryWithFakeAuditLog();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/traffic/live");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_Fines_Without_Token_Should_Return_401()
    {
        var factory = CreateFactoryWithFakeAuditLog();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/fines/list");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private WebApplicationFactory<Program> CreateFactoryWithFakeAuditLog()
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(IAuditLogRepository));
                services.AddSingleton<IAuditLogRepository>(new FakeAuditLogRepository());
            });
        });
    }

    private sealed class FakeAuditLogRepository : IAuditLogRepository
    {
        public Task CreateAsync(RequestAuditLog log)
        {
            return Task.CompletedTask;
        }
    }
}
