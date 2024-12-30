using System.Text.RegularExpressions;
using Hydro;

namespace justdoit.Pages.Shared.Components;

public class HydroLink : HydroView
{
    // todo: replace with better url check
    private static Regex url_regex = new Regex(
        @"https:(www)?//\w+\.\w+/.*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public bool is_valid => url_regex.IsMatch(url);
    public string url { get; set; } = string.Empty;
}
