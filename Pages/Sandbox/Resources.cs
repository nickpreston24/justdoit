using System.Reflection;
using System.Text;
using CodeMechanic.Types;

namespace justdoit.Pages.Sandbox;

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

        public static string GetSqlFile(string filename, bool debug = false)
        {
            if (filename.IsEmpty()) throw new ArgumentNullException(nameof(filename));
            string filepath = resources.FirstOrDefault(name => name.ToLower().Contains(filename));
            if (debug) Console.WriteLine("file path: \n" + filepath);
            using var stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream(filepath)!;
            using var streamReader = new StreamReader(stream, Encoding.UTF8);
            return streamReader.ReadToEnd();
        }

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