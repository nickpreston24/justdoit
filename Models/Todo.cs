using CodeMechanic.RegularExpressions;
using CodeMechanic.Types;

namespace justdoit.Models;

public class Todo
{
    public static Todo Create(string description)
    {
        Priority extracted_priority =
            description.Extract<Priority>(TodoPriorityRegex.Basic.CompiledRegex).SingleOrDefault()
            ?? "4";

        // todo:
        var now = DateTime.Now;
        // Due extracted_due_date = description
        //                              .Extract<Due>(TodoPriorityRegex.Basic.CompiledRegex)
        //                              .SingleOrDefault()
        //                          ?? now;

        // var extracted_status = description.Extract<TodoStatus>(@"(?<status>(pending))").SingleOrDefault() ?? "pending";

        // extracted_status.Dump("status");

        // due = extracted_due_date
        return new Todo()
        {
            content = description,
            priority = extracted_priority.Value,
            //		status = TodoStatus.Pending
        };
    }

    public bool is_recurring { set; get; } = false;
    public int id { get; set; } = -1;
    public string uri { get; set; } = string.Empty; // link to an individual record.
    public string content { get; set; } = string.Empty;
    public string description { set; get; }
    public string created_by { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
    public TodoStatus Status => status;

    public string[] labels =>
        content.Extract<TodoLabel>(@"(?<name>@\w+)").Select(label => label.name).ToArray(); // https://regex101.com/r/UPGuX2/1

    public int priority { get; set; } = 4;

    public DateTime due { get; set; } = DateTime.MinValue; // = Start.Add(Duration);
    public TimeSpan duration { get; set; } = TimeSpan.FromMinutes(15);
    public DateTime start { get; set; }
    public DateTime end { get; set; }

    public DateTime created_at { get; set; } = DateTime.MinValue;
    public DateTime last_modified { get; set; } = DateTime.MinValue;

    public string status_css
    {
        get
        {
            var options = new Dictionary<TodoStatus, string>()
            {
                [TodoStatus.Done] = "success",
                [TodoStatus.WIP] = "warning",
                [TodoStatus.Postponed] = "red-500",
                [TodoStatus.Pending] = "info",
                [TodoStatus.Unknown] = "red-500",
            };

            var found = options.TryGetValue(status, out var value);
            // Console.WriteLine("value is " + value);
            return found ? value : throw new Exception($"status '{status}' found");
        }
    }

    public string priority_css
    {
        get
        {
            var value = priority;
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

public static class TodoExtensions
{
    //...
}

public class Due
{
    public string date { get; set; } = string.Empty;
    public string due_string { get; set; }

    // [JsonProperty("string")] public string @string { get; set; }

    // public string friendly {get;set;} = "Feb 12";
    public string lang { get; set; } = "en";
    public string is_recurring { get; set; } = "false";

    public DateTime datetime => date.ToDateTime(fallback: DateTime.MinValue).Value;

    public string friendly_date => datetime.ToFriendlyDateString();
    public string humanized_age => datetime.HumanizeAge();
    public string humanized => datetime.Humanize().ToMaybe().IfNone("Unknown");
}
