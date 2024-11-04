using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace justdoit.Pages.Admin;

public class Index : PageModel
{
    private static int total_calls = 0;

    public void OnGet()
    {
        Console.WriteLine("onget()");
        // todo: load sprocs using CodeMechanic.MySql sproc repository...
    }

    public async Task<IActionResult> OnGetSprocs(bool debug = false)
    {
        return Content("foo");
    }

    public async Task<IActionResult> OnGetViews(bool debug = false)
    {
        return Content("foo");
    }

    public async Task<IActionResult> OnGetRunSproc(string sprocname, bool debug = false)
    {
        return Content($"{sprocname}");
    }
}
