using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

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
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/traffic/live");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_Fines_Without_Token_Should_Return_401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/fines/list");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
