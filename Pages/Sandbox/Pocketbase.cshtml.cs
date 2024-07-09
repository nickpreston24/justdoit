using CodeMechanic.Diagnostics;
using CodeMechanic.RegularExpressions;
using Dapper;
// using justdoit.pb;
// using justdoit.pb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace justdoit.Pages.Sandbox;

public class Pocketbase : PageModel
{
    [BindProperty] public string Content { get; set; } = string.Empty;

    //
    // private static CollectionTodos todos;
    private static List<MySqlTodo> mysql_todos = new();
    public List<MySqlTodo> MysqlTodos => mysql_todos;

    // public CollectionTodos Todos => todos;

    public void OnGet()
    {
        Console.WriteLine(nameof(OnGet));
    }

    public async Task<IActionResult> OnGetRemoveTodo(int id = -1)
    {
        Console.WriteLine(nameof(OnGetRemoveTodo));
        Console.WriteLine(id);


        var connectionString = SQLConnections.GetMySQLConnectionString();

        using var connection = new MySqlConnection(connectionString);

        string query = @"
            delete from todos where id = @id
        ";

        var rows = await connection.ExecuteAsync(query, new { id = id });
        Console.WriteLine($"{rows} affected.");
        return Content($"<span class='ml-4 alert h-8 alert-success'>Todo {id} deleted!</span>");
    }


    public async Task<IActionResult> OnPostAddTask()
    {
        try
        {
            Console.WriteLine(nameof(OnPostAddTask));
            Console.WriteLine("task:>>\n" + Content);

            var todo = new MySqlTodo()
            {
                content = Content
            };

            // await SaveToPocketBase(todo);
            await SaveToMySQL(todo);

            return Content($"<p>{Content}...</p>");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            return Partial("_Alert", exception);
        }
    }

    private async ValueTask SaveToMySQL(MySqlTodo todo)
    {
        Console.WriteLine(nameof(SaveToMySQL));
        var connectionString = SQLConnections.GetMySQLConnectionString();

        using var connection = new MySqlConnection(connectionString);

        string insert_query =
            @"insert into todos (content, priority) values (@content, @priority)";

        var extracted_priority = todo.content
            .Extract<Priority>(TodoPriorityRegex.Basic.CompiledRegex)
            // .Dump("priori incantum")
            .SingleOrDefault();

        // extracted_priority.Dump(nameof(extracted_priority));

        var results = await Dapper.SqlMapper
            .ExecuteAsync(connection, insert_query,
                new
                {
                    content = todo.content,
                    priority = extracted_priority?.Value ?? 4
                });

        Console.WriteLine($"logged {results} log records.");
    }

    //
    // private async Task<bool> SaveToPocketBase(Todo todo)
    // {
    //     Console.WriteLine(nameof(SaveToPocketBase));
    //     var myApp = new AcmeApplication(Environment.GetEnvironmentVariable("PB_REMOTE_URL"), "Acme");
    //
    //     var myData = myApp.Data;
    //
    //     // TODO: add create permissions, then re-run the pb generate... (ugh...)
    //     // todos.Add(newTask);
    //     return true;
    // }
    //
    public async Task<IActionResult> OnGetMySqlTodos(string content, string contentType)
    {
        Console.WriteLine(nameof(OnGetMySqlTodos));
        try
        {
            mysql_todos = await GetTodosFromMySQL();

            return Partial("_MySqlTodoTree", this);
        }
        catch (Exception exception)
        {
            Console.WriteLine("message:>> " + exception.Message);
            return Partial("_Alert", exception);
        }
    }

    // public async Task<IActionResult> OnGetPbTodos(string content, string contentType)
    // {
    //     Console.WriteLine(nameof(OnGetPbTodos));
    //     try
    //     {
    //         GetTodosFromPocketbase();
    //         return Partial("_TodoTree", this);
    //     }
    //     catch (Exception exception)
    //     {
    //         Console.WriteLine("message:>> " + exception.Message);
    //         return Partial("_Alert", exception);
    //     }
    // }
    //
    private async Task<List<MySqlTodo>> GetTodosFromMySQL()
    {
        Console.WriteLine(nameof(GetTodosFromMySQL));
        var connectionString = SQLConnections.GetMySQLConnectionString();

        using var connection = new MySqlConnection(connectionString);

        string query = @"
            select id, content, created_at, due, status, priority
            from todos;
            # order by priority desc;
        ";

        var results = (await connection.QueryAsync<MySqlTodo>(query)).ToList();
        results.Dump(nameof(results));
        return results;
    }

    // private static void GetTodosFromPocketbase()
    // {
    //     Console.WriteLine(nameof(GetTodosFromPocketbase));
    //     var myApp = new AcmeApplication(Environment.GetEnvironmentVariable("PB_REMOTE_URL"), "Acme");
    //
    //     var myData = myApp.Data;
    //     todos = myData.TodosCollection;
    //
    //     foreach (var todo in todos)
    //     {
    //         Console.WriteLine($"{todo.Id} ({todo.Content}): {todo.Url}");
    //     }
    // }
}

public record Priority
{
    public string raw_text { get; set; } = string.Empty; // e.g. p1
    public string friendly_name => $"Priority {Value}"; // e.g. 'Priority 1'
    public int Value { get; set; } = -1;
}

public record MySqlTodo
{
    public int id { get; set; } = -1;
    public string content { get; set; } = string.Empty;
    public string created_by { get; set; } = string.Empty;
    public int priority { get; set; } = -1;

    public DateTime due { get; set; } = DateTime.MinValue;
    public DateTime created_at { get; set; } = DateTime.MinValue;
    public DateTime last_modified { get; set; } = DateTime.MinValue;
}