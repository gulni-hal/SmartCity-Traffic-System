using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Fine.Api.Middleware;

public class InternalOnlyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public InternalOnlyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration; // appsettings.json'dan gizli şifreyi okumak için
    }

    public Task InvokeAsync(HttpContext context)
    {
        // TDD Red Aşaması: Bilerek patlatıyoruz!
        throw new NotImplementedException();
    }
}