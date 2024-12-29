using CodeMechanic.Diagnostics;
using CodeMechanic.RegularExpressions;
using CodeMechanic.Shargs;
using CodeMechanic.Types;
using Dapper;
using justdoit;
using MySql.Data.MySqlClient;
using Sharprompt;
using ILogger = Serilog.ILogger;

public class TodosService : QueuedService
{
    private readonly PushbulletService pushbullet;
    private readonly IArgsMap arguments;
    private readonly ILogger logger;

    public TodosService(IArgsMap arguments, ILogger logger)
    {
        this.arguments = arguments;
        this.logger = logger;

        // (bool create_new, string cmd_value) = arguments.WithCommand("new"); // todo: needs a way to gracefully handle the command not being there.
        // Console.WriteLine("cmd value :>> " + cmd_value);

        bool create_new_todo = arguments.HasCommand("new");
        if (create_new_todo)
            steps.Add(CreateNewTodo);
    }

    private async Task CreateNewTodo()
    {
        bool debug = arguments.HasFlag("--debug");

        string content = Prompt.Input<string>("Enter your new todo details");

        if (debug)
            Console.WriteLine("you entered content :>> " + content);

        var todo = new Todo();
        int rows = await InsertRow(todo);

        todo.Dump($"{rows} todos created");
    }

    public async Task<List<Todo>> GetAll()
    {
        var connectionString = SqlConnections.GetMySQLConnectionString();
        using var connection = new MySqlConnection(connectionString);
        string select_query = TodosQuery.AvailableTodos.Query;
        var todos = (await connection.QueryAsync<Todo>(select_query.Trim())).ToList();
        return todos;
    }

    public Task<List<Todo>> Search(Todo search)
    {
        throw new NotImplementedException();
    }

    public Task<Todo> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<int> Create(Todo todo)
    {
        var results = await InsertRow(todo);
        return results;
    }

    public Task Update(int id, Todo model)
    {
        throw new NotImplementedException();
    }

    public async Task<int> Delete(int id)
    {
        Console.WriteLine(id);
        using var connection = SqlConnections.CreateConnection();

        string query =
            @"
            delete from todos where id = @id
        ";

        var rows = await connection.ExecuteAsync(query, new { id = id });
        return rows;
    }

    public async Task<int> GetRowCount()
    {
        string query =
            @"
                        select count(id)
                        from todos;";

        using var connection = SqlConnections.CreateConnection();
        var rows = await connection.ExecuteAsync(query);
        return rows;
    }

    public async Task<List<string>> FindTables()
    {
        using var connection = SqlConnections.CreateConnection();

        // todo: you've implemented this somewhere else already.  Go find it and upload it as a myget, then call it here.

        // var tables = await connection.QueryAsync<SQLiteTableInfo>("SELECT * FROM sqlite_master WHERE type='table'");
        // var tableNames = tables.Dump("tables found");
        // return tableNames.ToList();

        return new List<string>(0);
    }

    public async Task<List<Todo>> SendRandom()
    {
        var all = (await this.GetAll());
        var random_todo = all.TakeFirstRandom();

        string message =
            random_todo.content
            + $" \n Find it here: https://justdoit.up.railway.app/todos/{random_todo.id}";

        string title = "justdoit";
        pushbullet.Send(title, message);
        Console.WriteLine($"Sent todo {random_todo.id} to your phone!");
        return random_todo.AsList();
    }

    private async Task<int> InsertRow(Todo todo, bool debug = false)
    {
        try
        {
            using var connection = SqlConnections.CreateConnection();
            string insert_query = TodosQuery.CreateOne.Query;

            var extracted_priority = todo
                .content.Extract<Priority>(TodoPriorityRegex.Basic.CompiledRegex)
                .SingleOrDefault();

            if (debug)
                extracted_priority.Dump(nameof(extracted_priority));

            var results = await Dapper.SqlMapper.ExecuteAsync(
                connection,
                insert_query,
                new
                {
                    content = todo.content,
                    priority = extracted_priority?.Value ?? 4,
                    status = todo.status,
                    due = todo.due,
                }
            );

            return results;
        }
        catch (Exception e)
        {
            logger.Error(e.ToString());
            throw;
        }
    }
}

public class TodosQuery : SqlQuery
{
    public static TodosQuery CreateOne = new TodosQuery(
        1,
        nameof(CreateOne),
        @"insert into todos (content, priority, status, due) values (@content, @priority, 'pending', @due)"
    );

    public static TodosQuery AvailableTodos = new TodosQuery(
        2,
        nameof(AvailableTodos),
        @"select id, content, status, priority, due
                        from AvailableTodos;"
    );

    public TodosQuery(int id, string name, string query)
        : base(id, name, query) { }
}

public class SqlQuery : Enumeration
{
    public SqlQuery(int id, string name, string query)
        : base(id, name)
    {
        this.Query = query;
    }

    public string Query { get; private set; }
}


// public interface ITodosRepository
// {
//     Task<List<Todo>> GetAll();
//     Task<List<Todo>> Search(Todo search);
//     Task<Todo> GetById(int id);
//     Task<int> Create(params Todo[] model);
//     Task Update(int id, Todo model);
//     Task<int> Delete(int id);
//     Task<int> GetRowCount();
// }
