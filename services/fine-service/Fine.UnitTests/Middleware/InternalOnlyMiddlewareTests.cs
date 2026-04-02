using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Xunit;
using Fine.Api.Middleware;

namespace Fine.UnitTests.Middleware;

public class InternalOnlyMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_Should_Return_403_When_Header_Is_Missing()
    {
        // Arrange (Hazırlık)
        var context = new DefaultHttpContext();
        // İstekte bilerek gizli şifre (Header) göndermiyoruz.

        // Sahte bir appsettings.json yapılandırması oluşturuyoruz
        var inMemorySettings = new Dictionary<string, string?> { { "InternalSecret", "SuperGizliSifre123" } };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

        RequestDelegate next = (HttpContext hc) => Task.CompletedTask;
        var middleware = new InternalOnlyMiddleware(next, configuration);

        // Act (Eylem)
        await middleware.InvokeAsync(context);

        // Assert (Doğrulama)
        // Beklentimiz: Şifre olmadığı için sistemin 403 Forbidden (Yasak) dönmesi
        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }
    [Fact]
    public async Task InvokeAsync_Should_Call_Next_When_Header_Is_Correct()
    {
        // Arrange
        var context = new DefaultHttpContext();
        //Doğru şifreyi header'a ekliyoruz
        context.Request.Headers["X-Internal-Secret"] = "SuperGizliSifre123";

        var inMemorySettings = new Dictionary<string, string?> { { "InternalSecret", "SuperGizliSifre123" } };
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

        bool nextCalled = false;
        RequestDelegate next = (HttpContext hc) =>
        {
            nextCalled = true; // İstek kapıdan içeri girdiyse burası true olacak
            return Task.CompletedTask;
        };

        var middleware = new InternalOnlyMiddleware(next, configuration);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled); // Middleware'in engellemeyip 'next'e geçtiğini doğruluyoruz
    }
}