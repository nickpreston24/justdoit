namespace justdoit.Models;

public abstract class Sort
{
    public Sort()
    {
        // If dev instantiates this, then always enable, because it makes sense.
        Enabled = true;
    }

    public virtual bool Enabled { set; get; } = false;
    public virtual SortDirection Direction { get; set; } = SortDirection.Ascending;
}