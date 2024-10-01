// using CodeMechanic.RegularExpressions;  NOTE: importing this dll breaks microsoft.  I don't know why.  I think it's because MS somehow interprets 'regularexpressions' as a duplicate dll.

namespace justdoit.Models;


public class TodoPriorityRegex : RegexEnumBase
{
    public static TodoPriorityRegex Basic =
        new TodoPriorityRegex(1, nameof(Basic), @"(?<raw_text>(priority\s*|p)(?<Value>[1-4]))",
            "https://regex101.com/r/twefSL/1"); // return that part to autozone tomorrow p1

    protected TodoPriorityRegex(int id, string name, string pattern, string uri = "") : base(id, name, pattern, uri)
    {
    }
}
