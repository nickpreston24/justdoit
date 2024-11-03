using CodeMechanic.Diagnostics;
using CodeMechanic.FileSystem;
using Coravel.Invocable;
using Shargs;

public class SendNotifications : IInvocable
{
    private readonly PushbulletService pushbullet;
    private readonly ArgsMap arguments;

    public SendNotifications(ArgsMap arguments, PushbulletService pushbulletService)
    {
        this.pushbullet = pushbulletService;
        this.arguments = arguments;
    }

    public async Task Invoke()
    {
        try
        {
            Console.WriteLine("invoke");
            string personal_dir = Directory
                .GetCurrentDirectory()
                .AsDirectory()
                .GoUpToDirectory("personal")
                .FullName;

            var tpot_links = new Grepper()
            {
                RootPath = personal_dir,
                Recursive = true,
                FileSearchMask = "*.*",
                FileSearchLinePattern = TpotPattern.Link.Pattern,
            }.GetMatchingFiles();

            tpot_links.Take(5).Dump("links found");

            Console.WriteLine("sending scriptures to your phone ...");
            string message =
                @"This is justdoit. Go to <a href='https://justdoit.up.railway.app/todos'></a> to get started.";
            string title = "justdoit";
            pushbullet.Send(title, message);
            // Thread.Sleep(1000);
            Console.WriteLine("Sent your phone!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
