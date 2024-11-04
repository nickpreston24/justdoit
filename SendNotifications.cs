using Coravel.Invocable;
using Shargs;

namespace justdoit;

public class SendNotifications : IInvocable
{
    private readonly PushbulletService pushbullet;
    private readonly ArgsMap arguments;
    private readonly ITodosRepository todos_repo;

    public SendNotifications(
        ArgsMap arguments,
        PushbulletService pushbulletService,
        ITodosRepository todos_repo
    )
    {
        Console.WriteLine("cotr.");
        this.pushbullet = pushbulletService;
        this.arguments = arguments;
        this.todos_repo = todos_repo;
    }

    public async Task Invoke()
    {
        try
        {
            // var all_todos = await todos_repo.GetAll();
            // var random_todo = all_todos.TakeFirstRandom();
            var random_todo = new Todo();

            string message =
                // @"This is justdoit. Go to <a href='https://justdoit.up.railway.app/todos'></a> to get started."
                // ;
                random_todo.content + $" \n Find it here: https://justdoit.up.railway.app/todos";
            string title = "justdoit";
            pushbullet.Send(title, message);
            Console.WriteLine($"Sent todo {random_todo.id} to your phone!");
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
// tpot_links.Take(5).Dump("links found");
