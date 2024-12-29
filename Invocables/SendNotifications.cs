using CodeMechanic.Shargs;
using Coravel.Invocable;
using ILogger = Serilog.ILogger;

namespace justdoit;

public class SendNotifications : IInvocable
{
    private readonly PushbulletService pushbullet;
    private readonly ArgsMap arguments;
    private readonly TodosService todos_service;
    private readonly ILogger logger;

    public SendNotifications(
        ArgsMap arguments,
        PushbulletService pushbulletService,
        TodosService todos_service,
        ILogger logger
    )
    {
        Console.WriteLine("cotr.");
        this.pushbullet = pushbulletService;
        this.arguments = arguments;
        this.todos_service = todos_service;
        this.logger = logger;
    }

    public async Task Invoke()
    {
        try
        {
            var random_todo = todos_service.SendRandom();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

// string personal_dir = Directory
//     .GetCurrentDirectory()
//     .AsDirectory()
//     .GoUpToDirectory("personal")
//     .FullName;
//
// var tpot_links = new Grepper()
// {
//     RootPath = personal_dir,
//     Recursive = true,
//     FileSearchMask = "*.*",
//     FileSearchLinePattern = TpotPattern.Link.Pattern,
// }.GetMatchingFiles();
//
// tpot_links.SendRandom(5).Dump("links found");
