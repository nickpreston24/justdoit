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

    private static List<MySqlTodo> mysql_todos = new();
    public List<MySqlTodo> MysqlTodos => mysql_todos;

    // private static CollectionTodos todos;
    // public CollectionTodos Todos => todos;

    public void OnGet()
    {
        Console.WriteLine(nameof(OnGet));
    }

    public async Task<IActionResult> OnGetSortTreadmill(int days_from_now = 7)
    {
        var start_time = DateTime.Now;
        var end_time = DateTime.Now.AddDays(days_from_now);
        var time_range = start_time.Subtract(end_time);

        var connectionString = SQLConnections.GetMySQLConnectionString();

        using var connection = new MySqlConnection(connectionString);

        string grab_query = @"
            select id, content, due, status, priority
            from todos;
        ";

        using var grabby_connection = new MySqlConnection(connectionString);

        var todos = (await connection.QueryAsync<MySqlTodo>(grab_query)).ToList();

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

        // todo: finish reseting the todos by the age of dueness.
        return Partial("_MySqlTodoTree", this);
    }

    public record AgeFromDueDate(DateTime due, int task_age_in_days);

    public async Task<IActionResult> OnGetCompleteTodo(int id = -1)
    {
        Console.WriteLine(nameof(OnGetCompleteTodo));
        Console.WriteLine(id);

        var connectionString = SQLConnections.GetMySQLConnectionString();

        using var connection = new MySqlConnection(connectionString);

        var current = mysql_todos.SingleOrDefault(x => x.id == id);
        string status = "";

        if (current.status.Dump("status").Equals(MySqlTodoStatus.Done.Name))
            status = MySqlTodoStatus.Pending.Name;

        else if (current.status.Equals(MySqlTodoStatus.Pending.Name))
            status = MySqlTodoStatus.Done.Name;


        status.Dump("new status");

        string query = @"
update todos
set status = @status
where id = @id";

        var rows = await connection.ExecuteAsync(query, new { id = id, status = status });
        Console.WriteLine($"{rows} affected.");

        // var updated_todo = mysql_todos.SingleOrDefault(t => t.id == id);

        // return Partial("_MySqlTodoTree", this);
        return Content($"<span class='ml-4 alert h-8 alert-success'>Todo {id} marked done.</span>");
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
                content = Content,
                status = "pending",
                due = DateTime.Now
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
            @$"insert into todos (content, priority, status, due) values (@content, @priority, '{MySqlTodoStatus.Pending.Name}', @due)";

        var extracted_priority = todo.content
            .Extract<Priority>(TodoPriorityRegex.Basic.CompiledRegex)
            // .Dump("priori incantum")
            .SingleOrDefault();

        extracted_priority.Dump(nameof(extracted_priority));

        var results = await Dapper.SqlMapper
            .ExecuteAsync(connection, insert_query,
                new
                {
                    content = todo.content,
                    priority = extracted_priority?.Value ?? 4,
                    status = todo.status,
                    due = todo.due
                });

        Console.WriteLine($"logged {results} log records.");
    }

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
    //         return Partial("_TodoTree", this);
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

public record Priority
{
    public string raw_text { get; set; } = string.Empty; // e.g. p1
    public string friendly_name => $"Priority {Value}"; // e.g. 'Priority 1'
    public int Value { get; set; } = -1;
    public static implicit operator Priority(string priority) => new Priority(priority);
}

public record MySqlTodo
{
    public int id { get; set; } = -1;
    public string content { get; set; } = string.Empty;
    public string created_by { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
    public MySqlTodoStatus Status => status;
    public int priority { get; set; } = -1;

    public DateTime due { get; set; } = DateTime.MinValue;
    public DateTime created_at { get; set; } = DateTime.MinValue;
    public DateTime last_modified { get; set; } = DateTime.MinValue;
}

public class Schedule
{
    // Set by LINQ:
    public DayOfWeek dayOfWeek { get; set; }
    public List<MySqlTodo> Todos { get; set; } = new();
}

public class MySqlTodoStatus : Enumeration
{
    public static MySqlTodoStatus Done = new MySqlTodoStatus(1, nameof(Done));
    public static MySqlTodoStatus Pending = new MySqlTodoStatus(2, nameof(Pending));
    public static MySqlTodoStatus WIP = new MySqlTodoStatus(3, nameof(WIP));
    public static MySqlTodoStatus Postponed = new MySqlTodoStatus(4, nameof(Postponed));

    public MySqlTodoStatus(int id, string name) : base(id, name)
    {
    }

    public static implicit operator MySqlTodoStatus(string status)
    {
        var found = MySqlTodoStatus.GetAll<MySqlTodoStatus>()
            .SingleOrDefault(x => x.Name.Equals(status, StringComparison.CurrentCultureIgnoreCase));
        return found;
    }
}