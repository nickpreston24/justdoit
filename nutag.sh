# create a new hydroview and tag helper
cd Pages/Components
cat >"$1".cshtml <<EOF
<div>
    @Model.Slot()
</div>
EOF
cat >"$1".cshtml.cs <<EOF
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Hydro;

[HtmlTargetElement("$1")]
public class $1 : HydroView
{
    // your props here
}
EOF
