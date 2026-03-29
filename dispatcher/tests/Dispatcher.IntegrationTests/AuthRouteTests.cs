using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

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
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/auth/login");

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
