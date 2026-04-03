using System.Net;
using Dispatcher.Application;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dispatcher.IntegrationTests;

public class AuthRouteTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthRouteTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Auth_Route_Without_Token_Should_Not_Return_401()
    {
        var factory = CreateFactoryWithFakeAuditLog();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/auth/login");

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_Auth_Route_Without_Token_Should_Be_Public()
    {
        var factory = CreateFactoryWithFakeAuditLog();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/auth/register");

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
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

    [Fact]
    public async Task Post_Logout_Route_Without_Token_Should_Not_Be_Blocked_By_Dispatcher()
    {
        var factory = CreateFactoryWithFakeAuditLog();
        var client = factory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/logout");
        var response = await client.SendAsync(request);

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

}
