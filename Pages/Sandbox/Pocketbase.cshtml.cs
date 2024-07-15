using CodeMechanic.Diagnostics;
using CodeMechanic.RegularExpressions;
using CodeMechanic.Types;
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
    [BindProperty] public string Query { get; set; } = string.Empty;
    private ITodosRepository db;

    private static List<Todo> todos = new();
    public List<Todo> Todos => todos;

    public Pocketbase(ITodosRepository db)
    {
        this.db = db;
    }

    // private static CollectionTodos todos;
    // public CollectionTodos Todos => todos;
    //
    // public void OnGet()
    // {
    //     Console.WriteLine(nameof(OnGet));
    // }

    public async Task<IActionResult> OnGetSortedTreadmill(int days_from_now = 7, string viewname = "_TodoistTasksTable")
    {
        Console.WriteLine(nameof(OnGetSortedTreadmill));
        if (viewname.IsEmpty()) throw new ArgumentNullException(nameof(viewname));

        todos = await db.GetAll();
        // todos = new List<Todo>()
        // {
        //     new Todo()
        //     {
        //         content = "Treadmill test @mmi",
        //         status = TodoStatus.Pending.ToString()
        //     }
        // };

        // todo: finish reseting the todos by the age of dueness.
        return Partial(viewname, this);
    }


    [Obsolete(
        "This was just a test.  I really want the scheduling to be must smarter and the sql to be easily updateable")]
    public async Task<IActionResult> OnGetSortTreadmill(int days_from_now = 7)
    {
        var start_time = DateTime.Now;
        var end_time = DateTime.Now.AddDays(days_from_now);
        var time_range = start_time.Subtract(end_time);

        todos =  await db.GetAll();

        /** var results = persons.GroupBy(
    p => p.PersonId, 
    p => p.car,
    (key, g) => new { PersonId = key, Cars = g.ToList() });
    */
        var my_schedule = todos
            // .Dump("original todos")
            .GroupBy(x => x.due.DayOfWeek, t => t, (key, t) => new Schedule()
            {
                dayOfWeek = key,
                Todos = t.ToList()
            })
            .ToList();

        var now = DateTime.Now;
        // my_schedule.Select(x => x.dayOfWeek).Dump("new schedule!");
        my_schedule
            .Select(x => x.Todos
                .Where(x => x.due != DateTime.MinValue)
                .Select(t =>
                    new AgeFromDueDate(t.due, now.Subtract(t.due).Days)
                    {
                    })).Dump("new schedule!");

        return Partial("_TodoTreadmill", this);
    }

    public record AgeFromDueDate(DateTime due, int task_age_in_days);

    public async Task<IActionResult> OnGetCompleteTodo(int id = -1)
    {
        Console.WriteLine(nameof(OnGetCompleteTodo));
        Console.WriteLine(id);

        var connectionString = SQLConnections.GetMySQLConnectionString();

        using var connection = new MySqlConnection(connectionString);

        var current = todos.SingleOrDefault(x => x.id == id);
        string status = "";

        if (current.status.Dump("status").Equals(TodoStatus.Done.Name))
            status = TodoStatus.Pending.Name;

        else if (current.status.Equals(TodoStatus.Pending.Name))
            status = TodoStatus.Done.Name;


        status.Dump("new status");

        string query = @"
update todos
set status = @status
where id = @id";

        var rows = await connection.ExecuteAsync(query, new { id = id, status = status });
        Console.WriteLine($"{rows} affected.");

        // var updated_todo = todos.SingleOrDefault(t => t.id == id);

        // return Partial("_TodoTreadmill", this);
        return Content($"<span class='ml-4 alert h-8 alert-success'>Todo {id} marked done.</span>");
    }

    public async Task<IActionResult> OnGetRemoveTodo(int id = -1)
    {
        int rows = await db.Delete(id);
        Console.WriteLine($"{rows} affected.");
        return Content($"<span class='ml-4 alert h-8 alert-success'>Todo {id} deleted!</span>");
    }

    public async Task<IActionResult> OnPostAddTask()
    {
        try
        {
            Console.WriteLine(nameof(OnPostAddTask));
            Console.WriteLine("task:>>\n" + Content);
            var todo = new Todo()
            {
                content = Content,
                status = "pending",
                due = DateTime.Now
            };

            // await SaveToPocketBase(todo);

            await db.Create(todo);

            return Content($"<p>{Content}...</p>");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            return Partial("_Alert", exception);
        }
    }


    public async Task<IActionResult> OnGetTodos(string content, string contentType)
    {
        Console.WriteLine(nameof(OnGetTodos));
        try
        {
            todos = await GetTodosFromMySQL();

            return Partial("_TodoTreadmill", this);
        }
        catch (Exception exception)
        {
            Console.WriteLine("message:>> " + exception.Message);
            return Partial("_Alert", exception);
        }
    }

    private async Task<List<Todo>> GetTodosFromMySQL()
    {
        Console.WriteLine(nameof(GetTodosFromMySQL));
        var connectionString = SQLConnections.GetMySQLConnectionString();

        using var connection = new MySqlConnection(connectionString);

        string query = @"
            select id, content, created_at, due, status, priority
            from todos;
            # order by priority desc;
        ";

        var results = (await connection.QueryAsync<Todo>(query)).ToList();
        // results.Dump(nameof(results));
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


    // public async Task<IActionResult> OnGetPbTodos(string content, string contentType)
    // {
    //     Console.WriteLine(nameof(OnGetPbTodos));
    //     try
    //     {
    //         GetTodosFromPocketbase();
    //         return Partial("_TodoTreadmill", this);
    //     }
    //     catch (Exception exception)
    //     {
    //         Console.WriteLine("message:>> " + exception.Message);
    //         return Partial("_Alert", exception);
    //     }
    // }
    //

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
}