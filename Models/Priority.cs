namespace justdoit;

public record Priority
{
    public string raw_text { get; set; } = string.Empty; // e.g. p1
    public string friendly_name => $"Priority {Value}"; // e.g. 'Priority 1'
    public int Value { get; set; } = -1;

    public static implicit operator Priority(string priority) => new Priority(priority);

    public static implicit operator Priority(int priority) => new Priority(priority.ToString());
}
