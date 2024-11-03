using System.Text;
using CodeMechanic.Types;

namespace Shargs;

/// <summary>
/// <seealso cref="ShargsPatternV2"/>
/// </summary>
public class Argument
{
    public bool IsInvalid => Value.NotEmpty() && Flag.IsEmpty(); // Values cannot exist without a flag.  This helps prevent cases like `dotnet run watch --urls ...` where there's two commands in a row, but `dotnet` is treated as a legit Value.  Can't have that!

    public string Flag { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
    public string raw_commands { get; set; } = string.Empty;

    public string[] commands =>
        raw_commands
        // .Dump("before split")
        .IsNotEmpty()
        && raw_commands.Length >= 1
            ? raw_commands.Split(' ')
            // .Dump("after split")
            : raw_commands.AsArray();

    public string raw_flags { get; set; } = string.Empty;

    public override string ToString()
    {
        return new StringBuilder().AppendLine($"{Flag}:{Value}").ToString();
    }

    public void Deconstruct(out bool exists, out string value)
    {
        exists = Flag.NotEmpty();
        value = Value;
    }
}
