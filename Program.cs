using CodeMechanic.FileSystem;
using Hydro.Configuration;
using justdoit.Services;

// using Lib.AspNetCore.ServerSentEvents;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<ITodosRepository, TodosRepository>();

builder.Services.AddRazorPages();
builder.Services.AddHydro();

// dependencies for server sent events
// credit: https://www.jetbrains.com/guide/dotnet/tutorials/htmx-aspnetcore/server-sent-events/
// builder.Services.AddServerSentEvents();
// builder.Services.AddHostedService<ServerEventsWorker>();


var app = builder.Build();
// Load and inject .env files & values
DotEnv.Load(debug: true);

// ConfigureMiddleware the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


// the connection for server events
// credit: https://www.jetbrains.com/guide/dotnet/tutorials/htmx-aspnetcore/server-sent-events/
// app.MapServerSentEvents("/rn-updates");

app.MapRazorPages();

app.ConfigureMiddleware();


app.UseHydro(builder.Environment);


app.Run();