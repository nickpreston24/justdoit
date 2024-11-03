using CodeMechanic.Diagnostics;
using CodeMechanic.RegularExpressions;
using CodeMechanic.Types;

namespace Shargs;

public class ArgsMap : IArgsMap
{
    private string flattened_args;
    private bool debug;
    private readonly string[] args;

    string[] IArgsMap.InitialArgs => args;

    public List<Argument> Arguments { get; set; } = new();

    public ArgsMap(params string[] args)
    {
        this.args = args;
        BuildArgsMap(args);
    }

    private void BuildArgsMap(string[] args)
    {
        flattened_args = string.Join(' ', args);
        // args.Dump("args");
        if (debug)
            Console.WriteLine("flattened args: " + flattened_args);

        var arguments = flattened_args.Extract<Argument>(ShargsPatternV2.Cli.CompiledRegex);
        this.Arguments = arguments;
        if (this.HasFlag("--debug"))
        {
            arguments.Dump("cli arguments v2 (internal)", ignoreNulls: true);
            arguments.Count.Dump("total arguments");
        }
    }

    private Argument GetMatchingCmd(string command_name)
    {
        var matching_command = Arguments.FirstOrDefault(a =>
            a.commands.Any(cmd => cmd.NotEmpty() && cmd.Equals(command_name))
        );

        return matching_command;
    }

    public bool HasCommand(string command_name)
    {
        var matching_commands = GetMatchingCmd(command_name);
        return matching_commands != null;
    }

    public Argument WithCommand(string command_name)
    {
        var matching_commands = GetMatchingCmd(command_name);
        return matching_commands;
    }

    /// <summary>
    /// Usage:
    ///     (var is_files_flag_checked ,var files) = options.Matching("-f", "--files");
    ///
    ///     files.Dump(); // "file1", "file2" ...
    /// </summary>
    public Argument WithFlags(params string[] flags)
    {
        return Arguments
            .FirstOrDefault(a => flags.Any(f => a.Flag == f))
            .ToMaybe()
            .Case(some: (arg) => arg, none: () => new Argument());
    }

    public bool HasFlag(params string[] flags)
    {
        var all_flag_names = Arguments.Where(a => a.Flag.NotEmpty()).Select(a => a.Flag).ToArray();

        if (flags.IsNullOrEmpty() || all_flag_names.IsNullOrEmpty())
            return false;

        return flags.Any(flag =>
            all_flag_names.Any(fn => fn.Equals(flag, StringComparison.OrdinalIgnoreCase))
        );
    }
}

public interface IArgsMap
{
    public bool HasCommand(string command_name);
    public Argument WithCommand(string command_name);
    string[] InitialArgs { get; }

    /// <summary>
    /// Usage:
    ///     (var is_files_flag_checked ,var files) = options.Matching("-f", "--files");
    ///
    ///     files.Dump(); // "file1", "file2" ...
    /// </summary>
    public Argument WithFlags(params string[] flags);

    public bool HasFlag(params string[] flags);
}

public static class ArgumentExtensions
{
    public static ArgsMap ToArgsMap(this string[] args) => new ArgsMap(args);

    public static bool HasAllOf(this IArgsMap arguments, params string[] args) =>
        args.All(a =>
            arguments
                .Dump("arguments in " + nameof(ToArgsMap))
                .HasCommand(a.Dump($"has command {a}?"))
            && arguments.HasFlag(a).Dump($"has flag {a}?")
        );

    public static bool HasAnyOf(this IArgsMap arguments, params string[] args) =>
        args.Any(a => arguments.HasCommand(a) || arguments.HasFlag(a));
}
