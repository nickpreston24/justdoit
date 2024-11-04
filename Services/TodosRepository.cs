using CodeMechanic.RegularExpressions;
using Dapper;
using justdoit;
using MySql.Data.MySqlClient;

public class TodosRepository : ITodosRepository
{
    public TodosRepository() { }

    public async Task<List<Todo>> GetAll()
    {
        var connectionString = SqlConnections.GetMySQLConnectionString();

        using var connection = new MySqlConnection(connectionString);

        string select_query =
            @"
            select id, content, status, priority, due
                        from AvailableTodos;
        ";

        using var grabby_connection = new MySqlConnection(connectionString);

        var todos = (await connection.QueryAsync<Todo>(select_query)).ToList();

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

    public async Task<int> Create(params Todo[] model)
    {
        return -1;
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

    private async Task<int> InsertRow(Todo todo)
    {
        try
        {
            using var connection = SqlConnections.CreateConnection();

            string insert_query =
                @$"insert into todos (content, priority, status, due) values (@content, @priority, 'pending', @due)";

            var extracted_priority = todo
                // .Dump("my todo added")
                .content.Extract<Priority>(TodoPriorityRegex.Basic.CompiledRegex)
                // .Dump("priori incantum")
                .SingleOrDefault();

            // extracted_priority.Dump(nameof(extracted_priority));

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
            Console.WriteLine(e);

            throw;
        }
    }
}

public interface ITodosRepository
{
    Task<List<Todo>> GetAll();
    Task<List<Todo>> Search(Todo search);
    Task<Todo> GetById(int id);
    Task<int> Create(params Todo[] model);
    Task Update(int id, Todo model);
    Task<int> Delete(int id);
    Task<int> GetRowCount();
    Task<List<string>> FindTables();
}
