using Hydro;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace justdoit.Pages.Components;

public class HydroSearch : HydroComponent
{
    public int Count { get; set; }

    public void Add()
    {
        Count++;
    }
    //
    // public string Query { get; set; } = string.Empty;
    // public string PartialName { get; set; } = string.Empty;
    // public string HxTarget { get; set; } = string.Empty;
    // public string PageName { get; set; } = "Index";
    // public string PageHandler { get; set; } = "OnGet";
}