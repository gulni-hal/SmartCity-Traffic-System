var builder = WebApplication.CreateBuilder(args);

// YARP ekle
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Auth middleware (þimdilik basit)
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.ContainsKey("Authorization"))
    {
        context.Response.StatusCode = 401;
        return;
    }

    await next();
});

// Reverse proxy baþlat
app.MapReverseProxy();

app.Run();

public partial class Program { }