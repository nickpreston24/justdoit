namespace justdoit.Models;

public class SortDirection : Enumeration
{
    public static SortDirection Descending = new(1, nameof(Descending));
    public static SortDirection Ascending = new(2, nameof(Ascending));

    public SortDirection(int id, string name)
        : base(id, name) { }
}
