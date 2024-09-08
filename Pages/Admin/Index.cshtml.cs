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

    public async Task<IActionResult> OnGetSprocs(string sproc_name = "", bool debug = false)
    {
        total_calls++;
        if (debug) Console.WriteLine(sproc_name);
        string filename = sproc_name + ".sql";
        return Content($"{sproc_name} {total_calls}");
    }
}