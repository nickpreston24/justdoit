using System.Text.RegularExpressions;

namespace Shargs;

public class ShargsPatternV2 : RegexEnumBase
{
    public static ShargsPatternV2 Cli = new ShargsPatternV2(
        1,
        nameof(Cli),
        @"(?<raw_commands>(([a-zA-Z]+)\s*){1,})|
(?<flag>-+[\w-]+)=?
(\s*(?<value>[a-zA-Z/\d.]+))? # Values after flags",
        uri: "https://regex101.com/r/TyNCZp/5"
    );

    public static ShargsPatternV2 Command = new ShargsPatternV2(
        2,
        @"^(?<command>(?!-).)*",
        nameof(Command)
    );

    public static ShargsPatternV2 Help = new ShargsPatternV2(
        3,
        nameof(Help),
        @"(?<raw_text>(?<flag>--?[\w-]+=?)(\s*(?<variable_name>[a-zA-Z/\d:\.]+))?)?",
        "https://regex101.com/r/hGTIO3/1"
    );

    protected ShargsPatternV2(int id, string name, string pattern, string uri = "")
        : base(id, name, pattern, uri)
    {
        this.CompiledRegex = new Regex(
            pattern,
            RegexOptions.Compiled
                | RegexOptions.Multiline
                | RegexOptions.IgnoreCase
                | RegexOptions.IgnorePatternWhitespace
        );
    }
}
