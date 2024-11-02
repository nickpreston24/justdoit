namespace justdoit.Models;

public class Schedule
{
    // Set by LINQ:
    public DayOfWeek dayOfWeek { get; set; }
    public List<Todo> Todos { get; set; } = new();
}
