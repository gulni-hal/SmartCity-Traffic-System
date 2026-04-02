using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Xunit;
using Auth.Api.Middleware;

namespace Auth.UnitTests.Middleware;

public class InternalOnlyMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_Should_Return_403_When_Header_Is_Missing()
    {
        var context = new DefaultHttpContext();
        var inMemorySettings = new Dictionary<string, string?> { { "InternalSecret", "Sifre123" } };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

        RequestDelegate next = (HttpContext hc) => Task.CompletedTask;
        var middleware = new InternalOnlyMiddleware(next, configuration);

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_Should_Call_Next_When_Header_Is_Correct()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Internal-Secret"] = "Sifre123";

        var inMemorySettings = new Dictionary<string, string?> { { "InternalSecret", "Sifre123" } };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

        bool nextCalled = false;
        RequestDelegate next = (HttpContext hc) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new InternalOnlyMiddleware(next, configuration);

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
    }
}