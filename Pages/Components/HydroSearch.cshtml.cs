using Hydro;

namespace justdoit.Pages.Components;

public class HydroSearch : HydroComponent
{
    public string Query { get; set; } = string.Empty;
    public string PartialName { get; set; } = string.Empty;
    public string HxTarget { get; set; } = string.Empty;
    public string PageName { get; set; } = "Index";
    public string PageHandler { get; set; } = "OnGet";
    public string Placeholder { get; set; } = "Search";
    public string InputType { get; set; } = "text";
}