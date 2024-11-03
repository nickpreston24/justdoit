namespace Shargs;

public class CliCommand
{
    public string command { get; set; } = string.Empty;
    public Argument[] Arguments { get; set; } = Array.Empty<Argument>();
}
