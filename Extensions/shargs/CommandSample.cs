namespace Shargs;

public class CommandSample : Enumeration
{
    public static CommandSample RazorHatGeneration = new CommandSample(
        1,
        nameof(RazorHatGeneration),
        @"--razorhat web -n dummyapp -o /home/nick/Desktop/projects/samples/sharpify -v"
    );

    public static CommandSample GrepSearch = new CommandSample(
        2,
        nameof(GrepSearch),
        @"--grep-search -i ."
    );

    public static CommandSample DotnetRun = new CommandSample(
        3,
        nameof(DotnetRun),
        @"dotnet run --no-self-contained -v q --os linux --no-restore --interactive"
    );

    public static CommandSample DotnetAddSln = new CommandSample(
        4,
        nameof(DotnetAddSln),
        @"--add-sln 3"
    ); //todo: Make this a command after you update the regex to include commands again.

    public static CommandSample Hello = new CommandSample(0, nameof(Hello), @"--hello");

    public static CommandSample DDGR_CLI = new CommandSample(
        5,
        nameof(DDGR_CLI),
        @"ddgr --json -N 10"
    );

    public static CommandSample SnapInstaller = new CommandSample(
        6,
        nameof(SnapInstaller),
        @"snap install --devmode --channel=foo --prefer --name=bar -v q"
    );

    private string args_text = string.Empty;

    public CommandSample(
        int id,
        string name,
        string args_text = "",
        CommandValidations validations = null
    )
        : base(id, name)
    {
        this.args_text = args_text;
        this.Validations = validations ?? new CommandValidations();
    }

    public CommandValidations Validations { get; set; } = new();

    public override string ToString()
    {
        return args_text;
    }

    // public List<Enumeration> GetAllCommandSamples()
    // {
    //     var cmds = Enumeration.GetAll<CommandSample>().ToList();
    //     return cmds;
    // }
}

public class CommandValidations
{
    public int expected_flag_count { get; set; } = -1;
    public int expected_command_count { get; set; } = -1;
    public int actual_flag_count { get; set; } = -1;
    public int actual_command_count { get; set; } = -1;

    public double percent_valid_commands =>
        (actual_command_count - expected_command_count) / expected_command_count * 100.00;

    public double percent_valid_flags =>
        (actual_flag_count - expected_flag_count) / expected_flag_count * 100.00;
}
