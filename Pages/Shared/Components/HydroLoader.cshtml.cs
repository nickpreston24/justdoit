using Hydro;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace justdoit.Pages.Shared.Components;

[HtmlTargetElement("loader")]
public class HydroLoader : HydroView
{
    public string id { get; set; } = string.Empty;
    public string message { get; set; } = string.Empty;
}
