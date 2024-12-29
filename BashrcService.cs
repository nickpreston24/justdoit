using CodeMechanic.Diagnostics;
using CodeMechanic.Shargs;
using Sharprompt;

/// <summary>
/// Reads and writes to .bashrc
/// </summary>
public class BashrcService : QueuedService
{
    private readonly IArgsMap arguments;

    public BashrcService(IArgsMap arguments)
    {
        this.arguments = arguments;
        bool update_bashrc = arguments.HasCommand("bashrc");
        if (!update_bashrc)
            return;

        var dict = new Dictionary<string, string>()
        {
            ["upload"] = "Copy .env -> .bashrc (up)",
            ["download"] = "Copy .bashrc -> .env (down)",
        };

        var choice = Prompt.Select("Do which?", dict);
        // choice.Dump("You chose:");

        BashrcOperation operation = choice.Key;

        if (operation.Equals(BashrcOperation.Upload))
            steps.Add(CopyEnvToBashrc);
    }

    private async Task CopyEnvToBashrc()
    {
        string cwd = Directory.GetCurrentDirectory();
        Console.WriteLine($"{nameof(cwd)}", cwd);
    }

    private async Task CopyBashrcToEnv() { }

    public class BashrcOperation : Enumeration
    {
        public static BashrcOperation Upload = new BashrcOperation(1, nameof(Upload));

        public static BashrcOperation Download = new BashrcOperation(2, nameof(Download));

        // etc.

        public BashrcOperation(int id, string name)
            : base(id, name) { }

        public static implicit operator BashrcOperation(string name)
        {
            return GetAll<BashrcOperation>()
                .SingleOrDefault<BashrcOperation>(
                    (Func<BashrcOperation, bool>)(
                        e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                    )
                );
        }
    }
}
