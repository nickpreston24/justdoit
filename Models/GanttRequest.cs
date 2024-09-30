namespace justdoit.Models;

public record GanttRequest
{
    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public DateTime Due { get; set; } // = Start.Add(Duration);

    public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(15);

    public override string ToString()
    {
        // return $"start: {Start.ValueOrDefault(DateTime.MinValue)}, end: {End.ValueOrDefault(DateTime.MaxValue)}";
        return $"start: {Start}\nend: {End}\ndue:{Due}\nduration:{Duration}";
    }
}
