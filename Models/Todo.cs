using CodeMechanic.RegularExpressions;
using pocketbase_csharp_sdk.Helper.Convert;

namespace justdoit.Models;

public record Todo
{
    public bool is_recurring { set; get; } = false;
    public int id { get; set; } = -1;
    public string uri { get; set; } = string.Empty; // link to an individual record.
    public string content { get; set; } = string.Empty;
    public string description { set; get; }
    public string created_by { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
    public TodoStatus Status => status;

    public string[] labels => content
        .Extract<TodoLabel>(@"(?<name>@\w+)")
        .Select(label => label.name)
        .ToArray(); // https://regex101.com/r/UPGuX2/1

    public int priority { get; set; } = 4;

    public DateTime due { get; set; } = DateTime.MinValue; // = Start.Add(Duration);
    public TimeSpan duration { get; set; } = TimeSpan.FromMinutes(15);
    public DateTime start { get; set; }
    public DateTime end { get; set; }

    public DateTime created_at { get; set; } = DateTime.MinValue;
    public DateTime last_modified { get; set; } = DateTime.MinValue;

    public string priority_css
    {
        get
        {
            var value = priority.ToInt();
            switch (value)
            {
                case 1:
                    return "error";
                case 2:
                    return "warning";
                case 3:
                    return "info";
                case 4:
                default:
                    return "ghost";
            }
        }
    }
}

public record TodoLabel
{
    public string name { get; set; } = string.Empty;
}