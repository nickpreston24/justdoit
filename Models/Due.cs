using CodeMechanic.Types;

namespace justdoit;

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
