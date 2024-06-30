using System.Net;
using System.Text;
using CodeMechanic.Diagnostics;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace justdoit.Pages.Sandbox;

public class Index : PageModel
{
    public void OnGet()
    {
    }

    public async Task<IActionResult> OnGetValkyrie()
    {
        // string url = "https://en.wikipedia.org/wiki/List_of_programmers";
        // string url = "https://en.wikipedia.org/wiki/List_of_SpongeBob_SquarePants_episodes";
        string url = "https://ammoseek.com/ammo/224-valkyrie";

        try
        {
            var response = await GetAmmoRecords(url, ms_delay: 2500);
            // var response = await CallUrl(url);
            // var response = await CallCurl(url);
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

    private async Task<List<AmmoseekRow>> GetAmmoRecords(string url, int ms_delay = 0)
    {
        if (ms_delay > 0)
            Thread.Sleep(ms_delay);

        var web = new HtmlWeb();
        // downloading to the target page
        // and parsing its HTML content
        var document = web.Load(url);
        foreach (HtmlNode table in document.DocumentNode.SelectNodes("//table"))
        {
            Console.WriteLine("Found: " + table.Id);
            foreach (HtmlNode row in table.SelectNodes("tr"))
            {
                Console.WriteLine("row");
                foreach (HtmlNode cell in row.SelectNodes("th|td"))
                {
                    Console.WriteLine("cell: " + cell.InnerText);
                }
            }
        }
        // var table = document.DocumentNode.SelectSingleNode("//table");
        // var tableRows = table.SelectNodes("tr");
        // var columns = tableRows[0].SelectNodes("th/text()");f
        // for (int i = 1; i < tableRows.Count; i++)
        // {
        //     for (int e = 0; e < columns.Count; e++)
        //     {
        //         var value = tableRows[i].SelectSingleNode($"td[{e + 1}]");
        //         Console.Write(columns[e].InnerText + ":" + value.InnerText);
        //     }
        //
        //     Console.WriteLine();
        // }

        // Console.WriteLine(document.Text);
        // var nodes = document.DocumentNode.SelectNodes(
        //     "");
        // var nodes = document.DocumentNode.SelectSingleNode("//table")
        //     .SelectNodes("tr")
        //     // .Where(node => !node.GetAttributeValue("class", "").Contains("tocsection"))
        //     .ToList();

        // Console.WriteLine("total nodes :>> " + nodes.Count);

        // // initializing the list of objects that will
        // // store the scraped data
        List<AmmoseekRow> rows = new List<AmmoseekRow>();
        // // looping over the nodes
        // // and extract data from them
        // foreach (var node in nodes)
        // {
        //     // node.Dump("node");
        //     // add a new Episode instance to
        //     // to the list of scraped data
        //     rows.Add(new AmmoseekRow()
        //     {
        //         retailer = HtmlEntity.DeEntitize(node.SelectSingleNode("th[1]").InnerText),
        //         // Title = HtmlEntity.DeEntitize(node.SelectSingleNode("td[2]").InnerText),
        //         // Directors = HtmlEntity.DeEntitize(node.SelectSingleNode("td[3]").InnerText),
        //         // WrittenBy = HtmlEntity.DeEntitize(node.SelectSingleNode("td[4]").InnerText),
        //         // Released = HtmlEntity.DeEntitize(node.SelectSingleNode("td[5]").InnerText)
        //     });
        // }

        // converting the scraped data to CSV...
        // storing this data in a db...
        // calling an API with this data...

        rows.Skip(1).FirstOrDefault().Dump("first row");
        return rows;
    }

    private static async Task<string> CallCurl(string url)
    {
        string response = await $"curl {url}".Bash(verbose: true);
        Console.WriteLine("curl response :>> \n" + response);
        return response;
    }

    private static async Task<string> CallUrl(string fullUrl)
    {
        HttpClient client = new HttpClient();
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
        client.DefaultRequestHeaders.Accept.Clear();
        var response = client.GetStringAsync(fullUrl);
        return await response;
    }

    // private List<AmmoseekRow> ParseHtml(string html)
    // {
    //     HtmlDocument htmlDoc = new HtmlDocument();
    //     htmlDoc.LoadHtml(html);
    //
    //     var nodes = htmlDoc.DocumentNode.Descendants("tr")
    //         // .Where(node => !node.GetAttributeValue("class", "").Contains("tocsection"))
    //         .ToList();
    //
    //     List<AmmoseekRow> rows = new List<AmmoseekRow>();
    //
    //     foreach (var node in nodes)
    //     {
    //         node.Dump("node");
    //         // if (link.FirstChild.Attributes.Count > 0)
    //         //     wikiLink.Add("https://en.wikipedia.org/" + link.FirstChild.Attributes[0].Value);
    //     }
    //
    //     return rows;
    // }

    private void WriteToCsv(List<string> links)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var link in links)
        {
            sb.AppendLine(link);
        }

        System.IO.File.WriteAllText("links.csv", sb.ToString());
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