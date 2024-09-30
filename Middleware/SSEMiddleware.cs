using justdoit.Services;
using Lib.AspNetCore.ServerSentEvents;

public static class SSEMiddleware
{
    public static WebApplicationBuilder AddServerSentEvents(this WebApplicationBuilder builder)
    {
        // dependencies for server sent events
        // credit: https://www.jetbrains.com/guide/dotnet/tutorials/htmx-aspnetcore/server-sent-events/
        builder.Services.AddServerSentEvents();
        builder.Services.AddHostedService<ServerEventsWorker>(); // sample
        return builder;
    }

    public static WebApplication MapEvents(this WebApplication app, params string[] patterns)
    {
        // the connection for server events
        // credit: https://www.jetbrains.com/guide/dotnet/tutorials/htmx-aspnetcore/server-sent-events/
        foreach (var pattern in patterns)
        {
            app.MapServerSentEvents(pattern);
        }

        return app;
    }
}
