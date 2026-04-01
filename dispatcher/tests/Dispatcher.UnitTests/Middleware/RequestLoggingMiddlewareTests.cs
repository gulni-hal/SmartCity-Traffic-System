using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Dispatcher.Api.Middleware;
using Dispatcher.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dispatcher.UnitTests.Middleware;

public class RequestLoggingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_Should_Write_Audit_Log_After_Request_Completes()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Method = "GET";
        context.Request.Path = "/api/traffic/live";
        context.Items["Username"] = "ali";
        context.Items["Role"] = "TrafficPolice";

        RequestDelegate next = async hc =>
        {
            hc.Response.StatusCode = StatusCodes.Status200OK;
            await Task.CompletedTask;
        };

        var repository = new FakeAuditLogRepository();
        var logger = new NullLogger<RequestLoggingMiddleware>();
        var middleware = new RequestLoggingMiddleware(next, logger, repository);

        await middleware.InvokeAsync(context);

        Assert.Single(repository.Logs);

        var log = repository.Logs[0];
        Assert.Equal("GET", log.Method);
        Assert.Equal("/api/traffic/live", log.Path);
        Assert.Equal(200, log.StatusCode);
        Assert.Equal("ali", log.Username);
        Assert.Equal("TrafficPolice", log.Role);
        Assert.Equal("traffic-service", log.TargetService);
    }

    private sealed class FakeAuditLogRepository : IAuditLogRepository
    {
        public List<RequestAuditLog> Logs { get; } = new();

        public Task CreateAsync(RequestAuditLog log)
        {
            Logs.Add(log);
            return Task.CompletedTask;
        }
    }
}

