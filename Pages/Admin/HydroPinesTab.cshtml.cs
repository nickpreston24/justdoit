using Hydro;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace justdoit.Pages.Admin;

[HtmlTargetElement("hydro-tabs")]
public class HydroPinesTab : HydroView
{
    public PinesTab[] Tabs { get; set; } =
        new[] { new PinesTab("foo", "bar"), new PinesTab("baz", "buu") };
}

public record PinesTab(string name, string viewname);