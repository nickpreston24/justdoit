using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dapper;
using justdoit.Models;

namespace justdoit.Pages.Sandbox;

public class EmbeddedSQL : PageModel
{
    public static List<Todo> todos = new();

    [BindProperty] public Stopwatch clock { get; set; } = new Stopwatch();

    // [BindProperty] 
    // public List<Todo> Todos => todos;

    public async Task<IActionResult> OnGetMoopsy(string sproc_name, string search_term)
    {
        clock.Start();
        // Console.WriteLine(sproc_name);
        // Console.WriteLine(search_term);
        Stopwatch resource_clock = new Stopwatch();
        resource_clock.Start();

        Resources.ListResourcesInAssembly(Resources.ThisAssembly);
        string sql_file_text = Resources.Embedded.SqlFile;

        resource_clock.Stop();
        Console.WriteLine(" resource find time (ms) :" + resource_clock.ElapsedMilliseconds);
        // Console.WriteLine("full text: \n" + sql_file_text);

        bool is_valid_proc = sql_file_text.Contains(sproc_name, StringComparison.OrdinalIgnoreCase);
        // is_valid_proc.Dump(nameof(is_valid_proc));

        if (is_valid_proc)
        {
            Stopwatch proc_clock = new Stopwatch();
            proc_clock.Start();
            // Console.WriteLine($"calling {sproc_name} with search: '{search_term}'");

            string sql = $"call search_todos('{search_term}');";
            // Console.WriteLine("running sql: \n" + sql);
            using var connection = SqlConnections.CreateConnection();
            todos = (await connection.QueryAsync<Todo>(sql, CommandType.StoredProcedure)).ToList();

            proc_clock.Stop();
            Console.WriteLine("time in ms for proc run: " + proc_clock.ElapsedMilliseconds);
        }

        clock.Stop();
        Console.WriteLine($"took {clock.ElapsedMilliseconds} milliseconds");
        clock.Reset();

        string title = "hi";
        string name = "nick";
        string message = "query success!";

        await
            $@"notify-send '{title}' '{message}' -a '{name}' -u normal -i face-smile".Bash(verbose: true);
        return Partial("_SampleTodoDataList", todos);
        // return Content("Hi");
    }
}

public static class Resources
{
    private static string[] resources;

    public static Assembly ThisAssembly
        => typeof(Resources).Assembly;

    public static void ListResourcesInAssembly(Assembly? assembly)
    {
        if (assembly is null)
        {
            Console.WriteLine("assembly is null");
            return;
        }

        resources = assembly.GetManifestResourceNames();
        Console.WriteLine("total resources found: " + resources.Length);
        if (resources.Length == 0)
            return;

        Console.WriteLine($"Resources in {assembly.FullName}");
        foreach (var resource in resources)
        {
            Console.WriteLine(resource);
        }
    }

    public static class Embedded
    {
        private static string sproc_name = "search_todos";
        private static bool debug = true;

        public static string SqlFile
        {
            get
            {
                // var info = Assembly.GetExecutingAssembly().GetName();
                // var ass_name = info.Name;
                // if (debug) Console.WriteLine("ass name:>> " + ass_name);
                string filename = $"{sproc_name}.sql";
                string filepath = resources.FirstOrDefault(name => name.ToLower().Contains(filename));
                // if (debug) 
                Console.WriteLine("file path: \n" + filepath);
                using var stream = Assembly
                    .GetExecutingAssembly()
                    .GetManifestResourceStream(filepath)!;
                using var streamReader = new StreamReader(stream, Encoding.UTF8);
                return streamReader.ReadToEnd();
            }
        }
    }
}