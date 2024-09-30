using CodeMechanic.Scraper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace justdoit.Pages.Sandbox;

public class Experimental : PageModel
{
    public void OnGet() { }

    public async Task<IActionResult> OnGetValkyrie()
    {
        // string url = "https://en.wikipedia.org/wiki/List_of_programmers";
        // string url = "https://en.wikipedia.org/wiki/List_of_SpongeBob_SquarePants_episodes";
        string url = "https://ammoseek.com/ammo/224-valkyrie";

        try
        {
            var response = await new HtmlScraperService().ScrapeHtmlTable<AmmoseekRow>(
                url,
                ms_delay: 2500
            );
            // var response = await CallUrl(url);
            // Console.WriteLine("content :>> \n" + response);
            // response.Dump("spongebob episodes");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Partial("_Alert", e);
        }

        return Content("rounds found.");
    }
}

public class Episode
{
    public string OverallNumber { get; set; }
    public string Title { get; set; }
    public string Directors { get; set; }
    public string WrittenBy { get; set; }
    public string Released { get; set; }
}

public class AmmoseekRow
{
    public string retailer { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public string brand { get; set; } = string.Empty;
    public string caliber { get; set; } = string.Empty;
    public string grains { get; set; } = string.Empty;
    public string limits { get; set; } = string.Empty;
    public string casing { get; set; } = string.Empty;
    public string is_new { get; set; } = string.Empty;
    public string price { get; set; } = string.Empty;
    public string rounds { get; set; } = string.Empty;
    public string price_per_round { get; set; } = string.Empty;
    public string shipping_rating { get; set; } = string.Empty;
    public string last_update { get; set; } = string.Empty; // last time Ammoseek updated this row.

    // Admin properties
    public string environment { get; set; } = ""; // Dev or prod
    public DateTimeOffset created_at { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset last_updated_at { get; set; } = DateTimeOffset.Now; // last time I updated this row!

    public string last_updated_by { get; set; } = string.Empty;
    public string created_by { get; set; } = string.Empty;
}
