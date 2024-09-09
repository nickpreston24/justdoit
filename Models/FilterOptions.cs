namespace justdoit.Models;

public class FilterOptions
{
    public List<string> excluded_labels { get; set; } = new()
        { "hobby", "workout", "buy", "cooking", "someday", "anytime", "coding", "computer", "guns", "wont_do" };

    public List<string> excluded_todo_ids { get; set; } = new();
    public List<string> excluded_projects { get; set; } = new() { "Food", "Recipes", "Coding", "Guns" };

    public bool distinct_ids_only { get; set; } = false;
    public bool distinct_content_only { get; set; } = false;
    public SortByDate sort_by_date { get; set; } = new();
    public SortByPriority sort_by_priority { get; set; } = new();
}