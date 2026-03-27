using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;


namespace Dispatcher.IntegrationTests;

public class UnauthorizedAccessTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UnauthorizedAccessTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Request_Without_Token_Should_Return_401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/traffic");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_Health_Without_Token_Should_Return_200()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Traffic_Without_Token_Should_Return_401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/services/traffic-service");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_Fines_Without_Token_Should_Return_401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/services/fine-service");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_Auth_Route_Without_Token_Should_Not_Return_401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/auth/login");

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

}


