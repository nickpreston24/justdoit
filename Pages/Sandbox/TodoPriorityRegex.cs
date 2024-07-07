using CodeMechanic.RegularExpressions;

namespace justdoit.Pages.Sandbox;

public class TodoPriorityRegex : RegexEnumBase
{
    public static TodoPriorityRegex Basic =
        new TodoPriorityRegex(1, nameof(Basic), @"(?<raw_text>(priority\s*|p)(?<Value>[1-4]))",
            "https://regex101.com/r/twefSL/1");

    protected TodoPriorityRegex(int id, string name, string pattern, string uri = "") : base(id, name, pattern, uri)
    {
    }
}