using System.Diagnostics;
using System.Runtime.CompilerServices;
using CodeMechanic.Types;
using CodeMechanic.Diagnostics;
using Dapper;
using justdoit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace justdoit.Pages.Todos;

public class Index : PageModel
{
    // private readonly ITodosRepository todo_repo;
    private bool debug;

    // public Index(ITodosRepository todosRepository)
    // {
    //     this.todo_repo = todosRepository;
    // }

    // public Index(IHttpClientFactory httpClientFactory)
    // {
    //     _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    // }

    public void OnGet()
    {
        this.debug = true;
    }

    private readonly IHttpClientFactory _httpClientFactory;
    [BindProperty(SupportsGet = true)] public Todo Todo { get; set; } = new Todo() { };

    [BindProperty(SupportsGet = true)] public string Email { get; set; } = string.Empty;
    [BindProperty(SupportsGet = true)] public string Content { get; set; } = string.Empty;

    public string[] ViewNames { get; set; } = new[] { "_TimeElapsedTable" };


    public async Task<IActionResult> OnGetArchive(int id = -2)
    {
        Console.WriteLine(id);

        string query = @"
update todos
set is_archived = 1
where id = @id;
";

        using var connection = SqlConnections.CreateConnection();
        int rows = await connection.ExecuteAsync(query, new { id = id });
        return Content($"Archived {rows} row!");
    }


    public async Task<IActionResult> OnGetAllTodos(string search_term, [CallerMemberName] string name = "")
    {
        // Console.WriteLine(nameof(OnGetAllTodos));
        if (search_term.NotEmpty())
            Console.WriteLine($"{name}:{search_term}");

        // Stopwatch watch = Stopwatch.StartNew();

        using var connection = SqlConnections.CreateConnection();

        var all_todos = (
                await connection.QueryAsync<Todo>(
                    @"                       
                        select id, content, status, priority, due
                        from AvailableTodos;"
                ))
            .ToList();

        // watch.Stop();
        // var elapsed = watch.Elapsed;
        return Partial("_TodoTable", all_todos);
    }

    /// <summary>
    /// for testing my railway api
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private async Task<string> GetMyInternalAPISampleTodos()
    {
        // get sample api todos:
        try
        {
            // var client = _httpClientFactory.CreateClient();
            var client = new HttpClient();
            var response = await client.GetAsync("https://justdoitapi-production.up.railway.app/todos");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    /// <summary>
    /// Renders a Mysql view requested from the frontend
    /// </summary>
    /// <param name="view_name"></param>
    /// <param name="result_type"></param>
    /// <param name="debug"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnGetRenderView(
        string view_name
        , Type result_type
        , bool debug = false
        , [CallerMemberName] string name = ""
    )
    {
        Console.WriteLine(name);
        try
        {
            Stopwatch watch = Stopwatch.StartNew();
            using var connection = SqlConnections.CreateConnection();
            var view_results = (await connection.QueryAsync(@"select id from TimeElapsed")).ToList();
            watch.Stop();
            var elapsed = watch.Elapsed;

            return Partial(view_name, view_results);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IActionResult> OnPostAddTodo(string content = "")
    {
        string query = @"insert into todos (content, status) values (@content, 'pending') ";

        Todo.Dump("adding new todo");
        Console.WriteLine("content = " + content);

        int rows = 0;
        using var connection = SqlConnections.CreateConnection();
        rows = await connection.ExecuteAsync(query, new Todo
        {
            content = Todo.content
        });

        return Content($"added {rows} rows.");
    }

    public async Task<IActionResult> OnPostRemoveTodo()
    {
        Console.WriteLine(nameof(OnPostRemoveTodo));
        int rows = -1;
        return Content($"removed {rows} rows.");
    }

    public async Task<IActionResult> OnGetBump(int id = -999, string days = "-7")
    {
        Console.WriteLine(id);
        return Content($"bumped {days} days");
    }

    public async Task<IActionResult> OnGetMarkDone(int id = 0, bool value = false)
    {
        Console.WriteLine(nameof(OnGetMarkDone));
        Console.WriteLine("Id: >> " + id);
        Console.WriteLine("toggle value: >> " + value);

        var now = DateTime.UtcNow;
        var last_modified = now;

        using var connection = SqlConnections.CreateConnection();
        string query =
            @"update todos 
            set status = 'done' 
            , last_modified = @last_modified
            where id = @id";

        int affected = await connection.ExecuteAsync(query, new
        {
            last_modified = last_modified,
            id = id
        });
        string message = $"{affected} row affected.";

        Console.WriteLine(message);
        return Content(message);


        // string html = @"""
        //                 <input
        //                     hx-get
        //                     hx-page='Index'
        //                     hx-page-handler='MarkDone'
        //                     type='checkbox' checked class='checkbox'/>
        //             """;
    }


    // public async Task<IActionResult> OnGetAllTodosV1(string search_term, [CallerMemberName] string name = "")
    // {
    //     Console.WriteLine(nameof(OnGetAllTodosV1));
    //     if (debug) Console.WriteLine($"{name}:{search_term}");
    //     Stopwatch watch = Stopwatch.StartNew();
    //     watch.Stop();
    //     var elapsed = watch.Elapsed;
    //     // var all_todos = await todo_repo.GetAll();
    //     var all_todos = new Todo().AsList();
    //     return Partial("_TodoTable", all_todos);
    // }

    public async Task<IActionResult> OnGetTimeElapsed(string search_term, [CallerMemberName] string name = "")
    {
        try
        {
            Console.WriteLine(nameof(OnGetTimeElapsed));
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

    // public async Task<IActionResult> OnGetAllTodosV2()
    // {
    //     var watch = Stopwatch.StartNew();
    //     string query = @"
    //                     select id, content, status, priority #, is_sample_data
    //                     from todos
    //                     where
    //                         content like 'test%'
    //                        or todos.is_sample_data = 1
    //                     ";
    //
    //     using var connection = SqlConnections.CreateConnection();
    //     // var todos = (await connection.QueryAsync(query)).ToArray();
    //
    //     // var todos = (await connection.QueryAsync("get_all_todos", CommandType.StoredProcedure)).ToArray();
    //     var all_todos = (
    //             await connection.QueryAsync<Todo>(
    //                 "select id, content from todos"
    //             ))
    //         // .Where(filters)
    //         .ToList();
    //
    //     watch.Stop();
    //     var ms = watch.ElapsedMilliseconds;
    //
    //     // return Content($"done.  found {todos.Length} todos, taking {ms} milliseconds");
    //     return Partial("_TodoTable", all_todos);
    // }
}