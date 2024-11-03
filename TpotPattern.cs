class TpotPattern : RegexEnumBase
{
    public static TpotPattern Link { get; } =
        new TpotPattern(1, nameof(Link), @"thepathoftruth.com");
    public static TpotPattern Scripture { get; } =
        new TpotPattern(1, nameof(Scripture), @"thepathoftruth.com\w+");

    protected TpotPattern(int id, string name, string pattern, string uri = "")
        : base(id, name, pattern, uri) { }
}
