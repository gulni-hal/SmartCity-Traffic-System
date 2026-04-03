using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading.Tasks;
using Dispatcher.Api.Middleware;
using Dispatcher.Application;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Dispatcher.UnitTests.Middleware;

public class AuthMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_Should_Skip_Auth_For_Logout_Route()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Path = "/api/auth/logout";
        context.Request.Method = "POST";

        var authService = new FakeAuthValidationService();
        var nextCalled = false;

        RequestDelegate next = hc =>
        {
            nextCalled = true;
            hc.Response.StatusCode = StatusCodes.Status200OK;
            return Task.CompletedTask;
        };

        var middleware = new AuthMiddleware(next);

        await middleware.InvokeAsync(context, authService);

        Assert.True(nextCalled);
        Assert.Equal(0, authService.CallCount);
    }

    [Fact]
    public async Task InvokeAsync_Should_Return_401_For_Expired_Token()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Path = "/api/traffic/live";
        context.Request.Method = "GET";
        context.Request.Headers["Authorization"] = "Bearer expired-token";

        var authService = new FakeAuthValidationService(new AuthValidationResult
        {
            IsValid = false
        });

        RequestDelegate next = hc =>
        {
            hc.Response.StatusCode = StatusCodes.Status200OK;
            return Task.CompletedTask;
        };

        var middleware = new AuthMiddleware(next);

        await middleware.InvokeAsync(context, authService);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_Should_Return_403_For_Observer_On_Fines_Route()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Path = "/api/fines/list";
        context.Request.Method = "GET";
        context.Request.Headers["Authorization"] = "Bearer valid-token";

        var authService = new FakeAuthValidationService(new AuthValidationResult
        {
            IsValid = true,
            Username = "ali",
            Role = "Observer"
        });

        RequestDelegate next = hc =>
        {
            hc.Response.StatusCode = StatusCodes.Status200OK;
            return Task.CompletedTask;
        };

        var middleware = new AuthMiddleware(next);

        await middleware.InvokeAsync(context, authService);

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_Should_Allow_Observer_On_Traffic_Route()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Path = "/api/traffic/live";
        context.Request.Method = "GET";
        context.Request.Headers["Authorization"] = "Bearer valid-token";

        var authService = new FakeAuthValidationService(new AuthValidationResult
        {
            IsValid = true,
            Username = "ali",
            Role = "Observer"
        });

        var nextCalled = false;

        RequestDelegate next = hc =>
        {
            nextCalled = true;
            hc.Response.StatusCode = StatusCodes.Status200OK;
            return Task.CompletedTask;
        };

        var middleware = new AuthMiddleware(next);

        await middleware.InvokeAsync(context, authService);

        Assert.True(nextCalled);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    private sealed class FakeAuthValidationService : IAuthValidationService
    {
        private readonly AuthValidationResult _result;

        public int CallCount { get; private set; }

        public FakeAuthValidationService()
        {
            _result = new AuthValidationResult
            {
                IsValid = true,
                Username = "ali",
                Role = "TrafficPolice"
            };
        }

        public FakeAuthValidationService(AuthValidationResult result)
        {
            _result = result;
        }

        public Task<AuthValidationResult> ValidateAsync(string token)
        {
            CallCount++;
            return Task.FromResult(_result);
        }
    }
}

