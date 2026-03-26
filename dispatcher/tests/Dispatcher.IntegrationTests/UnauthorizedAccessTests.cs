using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net;
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
}

