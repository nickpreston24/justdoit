using System.Diagnostics;
using System.Runtime.CompilerServices;
using Dapper;
using justdoit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace justdoit.Pages.Todos;

public class Index : PageModel
{
    private bool debug;

    public void OnGet()
    {
        this.debug = true;
    }

    public async Task<IActionResult> OnGetAllTodos(string search_term, [CallerMemberName] string name = "")
    {
        if (debug) Console.WriteLine($"{name}:{search_term}");
        Stopwatch watch = Stopwatch.StartNew();
        using var connection = SqlConnections.CreateConnection();
        var all_todos = (await connection.QueryAsync<Todo>(@"select id, content from todos")).ToList();
        watch.Stop();
        var elapsed = watch.Elapsed;
        // return Content($"total {all_todos.Count} took {elapsed.Milliseconds} ms");

        return Partial("_TodoTable", all_todos);
    }

    public async Task<IActionResult> OnGetTimeElapsed(string search_term, [CallerMemberName] string name = "")
    {
        try
        {
            if (debug) Console.WriteLine($"{name}:{search_term}");
            Stopwatch watch = Stopwatch.StartNew();
            using var connection = SqlConnections.CreateConnection();
            var time = (await connection.QueryAsync<TimeElapsed>(@"select id from TimeElapsed")).ToList();
            watch.Stop();
            var elapsed = watch.Elapsed;
            // return Content($"(mysql view call) total {time.Count} took {elapsed.Milliseconds} ms");

            return Partial("_TimeElapsedTable", time);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

public class TimeElapsed
{
    public string days_since_last_modification { get; set; } = string.Empty;
    public string days_old { get; set; } = string.Empty;
    public string days_until_due { get; set; } = string.Empty;
    public string due { get; set; } = string.Empty;
    public string created_at { get; set; } = string.Empty;
    public string last_modified { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
    public string priority { get; set; } = string.Empty;
    public string id { get; set; } = string.Empty;
}