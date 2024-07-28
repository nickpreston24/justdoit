using CodeMechanic.Diagnostics;
using justdoit.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace justdoit.Pages.Sandbox;

public class HydroExperiments : PageModel
{
    private static List<Todo> fake_todos = new();
    public List<Todo> FakeTodos => fake_todos;
    public void OnGet()
    {
    }

    public void OnGetSave(string content = "")
    {
        fake_todos.Add(
            new Todo() { content = content }
        );
    }

    public void OnGetSearch(string query = "")
    {
        var results = fake_todos.Where(t => t.content.Contains(query)).ToList();
        results.Dump(nameof(results));
    }
}