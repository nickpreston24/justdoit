using CodeMechanic.FileSystem;
using Coravel;
using Hydro.Configuration;
using justdoit;
using Serilog;
using Shargs;
using Log = Serilog.Log;

internal class Program
{
    static void Main(string[] args)
    {
        // Load and inject .env files & values
        DotEnv.Load(debug: false);

        Console.WriteLine("hello from justdoit!");
        var arguments = new ArgsMap(args);
        bool is_cli_mode = arguments.HasCommand("cli");
        bool is_web_mode = !is_cli_mode;

        if (is_cli_mode)
        {
            RunAsDaemon(args);
        }

        if (is_web_mode)
        {
            RunAsWebsite(args);
        }

        // arguments.Dump(nameof(arguments));
        // args.Dump(nameof(args));
        // Console.WriteLine($"is cli mode? {is_cli_mode}");
    }

    // NOTE: EXPERIMENTAL
    static void RunAsDaemon(string[] args)
    {
        var arguments = new ArgsMap(args);
        bool debug = arguments.HasFlag("--debug");

        Console.WriteLine("setting up Coravel...");
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddScheduler();

        builder.Services.AddSingleton(arguments);
        builder.Services.AddSingleton<PushbulletService>();
        builder.Services.AddSingleton<ITodosRepository>(new TodosRepository());

        builder.Services.AddTransient<SendNotifications>();

        var host = builder.Build();

        host.Services.UseScheduler(scheduler =>
        {
            if (debug)
                scheduler.Schedule(() => Console.WriteLine("It's alive! 🧟")).EveryFifteenSeconds();
            scheduler.Schedule<SendNotifications>().EveryMinute().Once();
        });

        host.Run();
        Console.WriteLine("DONE setting up Coravel...");
    }

    static void RunAsWebsite(string[] args)
    {
        try
        {
            Log.Information("starting server.");

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // builder.Services.AddTransient<ITodosRepository, TodosRepository>();

            builder.Services.AddRazorPages();
            builder.Services.AddHydro();

            builder.Host.UseSerilog(
                (context, loggerConfiguration) =>
                {
                    loggerConfiguration.WriteTo.Console();
                    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
                }
            );

            var app = builder.Build();

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
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "server terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
