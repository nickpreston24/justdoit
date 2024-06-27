using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace justdoit.Pages;

[BindProperties]
public class IndexModel : PageModel
{
    public string Email { get; set; } = string.Empty;
    public string CC { get; set; } = string.Empty;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostSignup()
    {
        Console.WriteLine(nameof(OnPostSignup));
        Console.WriteLine("email: " + Email);
        Console.WriteLine("credit: " + CC);
        return Partial("_SignupThankYou");
    }
}