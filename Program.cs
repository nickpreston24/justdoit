using CodeMechanic.FileSystem;
using Hydro.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddTransient<ITodosRepository, TodosRepository>();

builder.Services.AddRazorPages();
builder.Services.AddHydro();

var app = builder.Build();

// Load and inject .env files & values
DotEnv.Load(debug: false);

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

app.UseHydro(builder.Environment);

app.MapRazorPages();

// app.ConfigureMiddleware();

app.Run();

/*

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.MapRazorPages();

app.Run();

*/
