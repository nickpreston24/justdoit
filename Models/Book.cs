public record Book
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Id { get; set; } = -1;

    public override string ToString()
    {
        return $"'{Title}', by {Author}";
    }
}