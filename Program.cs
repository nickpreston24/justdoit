using CodeMechanic.FileSystem;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<ITodosRepository, TodosRepository>();

builder.Services.AddRazorPages();

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

app.MapRazorPages();

app.ConfigureMiddleware();

app.Run();