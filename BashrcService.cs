using System.Text;
using CodeMechanic.Bash;
using CodeMechanic.Diagnostics;
using CodeMechanic.FileSystem;
using CodeMechanic.RegularExpressions;
using CodeMechanic.Shargs;
using CodeMechanic.Types;
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

        if (arguments.HasCommand("list"))
            steps.Add(ListAllExportedVariables);
    }

    private async Task ListAllExportedVariables()
    {
        // get .env values:
        var dot_env_values = DotEnv.Load();

        var linux_vars = dot_env_values.ToDictionary(
            x => x.Left,
            y => Environment.GetEnvironmentVariable(y.Left)
        );

        linux_vars.Dump(nameof(linux_vars));
    }

    private async Task CopyEnvToBashrc()
    {
        string cwd = Directory.GetCurrentDirectory();
        // Console.WriteLine($"{nameof(cwd)}", cwd);

        // get .env values:
        var dot_env_values = DotEnv.Load();
        // dot_env_values.Dump(nameof(dot_env_values));

        // Generate updates to apply to .bashrc:
        string added_text = new StringBuilder()
            .AppendEach(
                dot_env_values,
                variable =>
                {
                    // e.g: `Foo="Bar"; export Foo`
                    // return variable.Left;
                    return $"{variable.Left}=\"{variable.Right}\"; export {variable.Left}";
                },
                delimiter: "\n"
            )
            .ToString();

        // Console.WriteLine($"{nameof(added_text)} \n" + added_text);

        // grab existing values from .bashrc

        string bashrc_filepath = (await "ls ~/.bashrc".Bash(verbose: false)).Trim();
        // Console.WriteLine($"{nameof(bashrc_filepath)} " + bashrc_filepath);

        string copy_output = await $"cp {bashrc_filepath} {cwd}".Bash();
        string current_dir_bashrc_path = Path.Combine(cwd, ".bashrc");
        bool local_copy_of_bashrc_exists = File.Exists(current_dir_bashrc_path);
        // Console.WriteLine($"{nameof(local_copy_of_bashrc_exists)} " + local_copy_of_bashrc_exists);

        string bashrc_text = File.ReadAllText(current_dir_bashrc_path);
        // Console.WriteLine($"{nameof(bashrc_text)}" + bashrc_text);

        var existing_exports = bashrc_text.Extract<BashrcExport>(
            BashrcFilePatterns.Exports.CompiledRegex
        );

        var added_exports = added_text.Extract<BashrcExport>(
            BashrcFilePatterns.Exports.CompiledRegex
        );

        // if(debug) existing_exports.Dump(nameof(existing_exports));
        // if(debug) added_exports.Dump(nameof(added_exports));

        // Diff existing with new:

        var diffed = added_exports.Except(existing_exports);

        // if(debug) diffed.Dump(nameof(diffed));

        // concat & save:

        string updated_text = new StringBuilder()
            .AppendEach(
                diffed,
                variable =>
                {
                    // e.g: `Foo="Bar"; export Foo`
                    return $"{variable.env_varname}=\"{variable.value}\"; export {variable.export_varname}";
                },
                delimiter: "\n"
            )
            .ToString();

        string updates_output = await $"echo '{updated_text}' >> .bashrc".Bash(verbose: true);
        Console.WriteLine("Done updating current working dir copy of .bashrc");
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
                    .SingleOrDefault(
                        (Func<BashrcOperation, bool>)(
                            enumeration =>
                                enumeration.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                        )
                    )
                ?? throw new Exception(
                    $"Could not find {nameof(BashrcOperation)} with name '{name}'"
                );
        }
    }

    public class BashrcExport
    {
        public string env_varname { get; set; } = String.Empty;
        public string value { get; set; } = String.Empty;
        public string export_varname { get; set; } = String.Empty;

        public override bool Equals(object? obj)
        {
            return this.ToString().Equals(obj.ToString(), StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return new StringBuilder()
                .Append($"{nameof(env_varname)}={env_varname};")
                .Append($"{nameof(value)}={value};")
                .Append($"{nameof(export_varname)}={export_varname};")
                .ToString();
        }
    }

    public class BashrcFilePatterns : RegexEnumBase
    {
        public static BashrcFilePatterns Exports = new BashrcFilePatterns(
            1,
            nameof(Exports),
            @"(?<env_varname>\w+)=""(?<value>.*?)"";\s*?export\s*?(?<export_varname>\w+)",
            "https://regex101.com/r/7vmhVi/2"
        );

        protected BashrcFilePatterns(int id, string name, string pattern, string uri = "")
            : base(id, name, pattern, uri) { }

        public static implicit operator BashrcFilePatterns(string name)
        {
            return GetAll<BashrcFilePatterns>()
                    .SingleOrDefault<BashrcFilePatterns>(
                        (Func<BashrcFilePatterns, bool>)(
                            e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                        )
                    )
                ?? throw new Exception(
                    $"Could not find {nameof(BashrcFilePatterns)} with name '{name}'"
                );
        }
    }
}
