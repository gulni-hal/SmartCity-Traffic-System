using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Dispatcher.Api.Middleware;
using Dispatcher.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dispatcher.UnitTests.Middleware;

public class AdminActionLoggingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_Admin_Post_Request_Should_Write_Admin_Log()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Method = "POST";
        context.Request.Path = "/api/fines/create";
        context.Items["Username"] = "adminUser";
        context.Items["Role"] = "Admin";

        RequestDelegate next = async hc =>
        {
            hc.Response.StatusCode = StatusCodes.Status200OK;
            await Task.CompletedTask;
        };

        var repository = new FakeAdminActionLogRepository();
        var logger = new NullLogger<AdminActionLoggingMiddleware>();
        var middleware = new AdminActionLoggingMiddleware(next, logger);

        await middleware.InvokeAsync(context, repository);

        Assert.Single(repository.Logs);

        var log = repository.Logs[0];
        Assert.Equal("adminUser", log.Username);
        Assert.Equal("Admin", log.Role);
        Assert.Equal("POST", log.Method);
        Assert.Equal("/api/fines/create", log.Path);
        Assert.Equal("fine-service", log.TargetService);
    }

    [Fact]
    public async Task InvokeAsync_Non_Admin_Request_Should_Not_Write_Admin_Log()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Method = "POST";
        context.Request.Path = "/api/fines/create";
        context.Items["Username"] = "ali";
        context.Items["Role"] = "TrafficPolice";

        RequestDelegate next = async hc =>
        {
            hc.Response.StatusCode = StatusCodes.Status200OK;
            await Task.CompletedTask;
        };

        var repository = new FakeAdminActionLogRepository();
        var logger = new NullLogger<AdminActionLoggingMiddleware>();
        var middleware = new AdminActionLoggingMiddleware(next, logger);

        await middleware.InvokeAsync(context, repository);

        Assert.Empty(repository.Logs);
    }

    [Fact]
    public async Task InvokeAsync_Admin_Get_Request_Should_Not_Write_Admin_Log()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Method = "GET";
        context.Request.Path = "/api/fines/list";
        context.Items["Username"] = "adminUser";
        context.Items["Role"] = "Admin";

        RequestDelegate next = async hc =>
        {
            hc.Response.StatusCode = StatusCodes.Status200OK;
            await Task.CompletedTask;
        };

        var repository = new FakeAdminActionLogRepository();
        var logger = new NullLogger<AdminActionLoggingMiddleware>();
        var middleware = new AdminActionLoggingMiddleware(next, logger);

        await middleware.InvokeAsync(context, repository);

        Assert.Empty(repository.Logs);
    }

    private sealed class FakeAdminActionLogRepository : IAdminActionLogRepository
    {
        public List<AdminActionLog> Logs { get; } = new();

        public Task CreateAsync(AdminActionLog log)
        {
            Logs.Add(log);
            return Task.CompletedTask;
        }
    }
}
