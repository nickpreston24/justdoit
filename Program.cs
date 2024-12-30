using System.Text;
using CodeMechanic.FileSystem;
using CodeMechanic.Shargs;
using Coravel;
using Hydro.Configuration;
using justdoit;
using Serilog;
using Serilog.Core;
using ILogger = Serilog.ILogger;

internal class Program
{
    static async Task Main(string[] args)
    {
        // Load and inject .env files & values
        DotEnv.Load(debug: false);

        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File(
                "/logs/justdoit.log",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true
            )
            .CreateLogger();

        var arguments = new ArgsMap(args);
        bool debug = arguments.HasFlag("--debug");

        bool get_random_todo = arguments.HasFlag("--random") && arguments.HasCommand("todo");

        (bool run_as_web, bool run_as_cli) = arguments.GetRunModes();

        if (debug)
            Console.WriteLine($"{nameof(run_as_web)}: {run_as_web}");

        if (debug)
            Console.WriteLine($"{nameof(run_as_cli)}: {run_as_cli}");

        bool is_daemon_mode = false;

        if (run_as_cli)
        {
            await RunAsCli(arguments, logger);
        }

        if (run_as_web)
        {
            RunAsWebsite(args, logger);
        }

        if (is_daemon_mode)
        {
            RunAsDaemon(args, logger);
        }
    }

    static async Task RunAsCli(IArgsMap arguments, Logger logger)
    {
        var services = CreateServices(arguments, logger);
        Application app = services.GetRequiredService<Application>();
        await app.Run();
    }

    // NOTE: EXPERIMENTAL
    static void RunAsDaemon(string[] args, ILogger logger)
    {
        logger.Information("running in daemon mode");

        var arguments = new ArgsMap(args);
        bool debug = arguments.HasFlag("--debug");

        if (debug)
            Console.WriteLine("setting up Coravel...");
        Console.OutputEncoding = Encoding.UTF8;

        var builder = Host.CreateApplicationBuilder(args);
        if (debug)
            Console.WriteLine("Adding Coravel Sheduler...");

        builder.Services.AddScheduler();

        if (debug)
            Console.WriteLine("Adding singletons...");

        builder.Services.AddSingleton(arguments);
        builder.Services.AddSingleton<ILogger>(logger);
        builder.Services.AddSingleton<PushbulletService>();
        builder.Services.AddSingleton<TodosService>();

        if (debug)
            Console.WriteLine("Adding transients...");

        builder.Services.AddTransient<SendNotifications>();

        if (debug)
            Console.WriteLine("Building host...");

        var host = builder.Build();

        host.Services.UseScheduler(scheduler =>
        {
            if (debug)
                Console.WriteLine("Hello from UseScheduler!");

            // if (debug)
            //     scheduler.Schedule(() => Console.WriteLine("It's alive! 🧟")).EveryFifteenSeconds();
            // scheduler.Schedule<SendNotifications>().EveryFifteenMinutes();
        });

        host.Run();
        Console.WriteLine("DONE setting up Coravel...");
    }

    static void RunAsWebsite(string[] args, Logger logger)
    {
        try
        {
            logger.Information("justdoit in web mode.");

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
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

            app.Run();
        }
        catch (Exception ex)
        {
            logger.Fatal(ex, "server terminated unexpectedly");
        }
        finally
        {
            logger.Dispose();
        }
    }

    private static ServiceProvider CreateServices(IArgsMap arguments, Logger logger)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton(arguments)
            .AddSingleton<ILogger>(logger)
            .AddSingleton<Application>()
            .AddSingleton<TodosService>()
            .AddScoped<BashrcService>()
            .AddSingleton<PushbulletService>()
            .BuildServiceProvider();

        return serviceProvider;
    }
}

internal class Person { }

public class Application
{
    private readonly ILogger logger;
    private readonly TodosService todos_service;
    private readonly BashrcService bashrc_service;

    public Application(ILogger logger, TodosService todos, BashrcService bashrcService)
    {
        this.logger = logger;
        this.todos_service = todos;
        this.bashrc_service = bashrcService;
    }

    public async Task Run()
    {
        await todos_service.Run();
        await bashrc_service.Run();
    }
}

// string person_pattern =
//     "https://regex101.com/r/Nzdp9E/1"; // https://regex101.com/r/Nzdp9E/1
// var person_regex = new Regex(person_pattern,
//     RegexOptions.Compiled | RegexOptions.IgnoreCase);
// string text =
//     "id=1234,firstname=nick,lastname=preston,dateofbirth=08-07-1989,gender=m,language=eng";
// var people = text.Extract<Person>(person_pattern);
// people.Dump(nameof(people)); // json
