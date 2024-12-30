using Hydro;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace justdoit.Pages.Shared.Components;

[HtmlTargetElement("modal")]
public class HydroModal : HydroView
{
    public string Title { get; set; } = String.Empty;
}
