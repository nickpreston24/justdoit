using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace justdoit.Pages;

[BindProperties]
public class IndexModel : PageModel
{
    public string Email { get; set; } = string.Empty;
    public string CC { get; set; } = string.Empty;

    private static int total_users = 0;
    public int TotalUsers = total_users;

    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnGetCountUsers()
    {
        var connectionString = SqlConnections.GetMySQLConnectionString();

        using var connection = new MySqlConnection(connectionString);

        // string query =
        //             @"
        // SELECT COUNT(email) AS `email_count`
        // FROM signups
        // GROUP BY email;
        // ";

        string query =
            @"
select distinct email, credit_card
from signups;";

        var rows = await connection.QueryAsync(query);
        Console.WriteLine(rows);

        return Partial("_UserCount", rows.ToList().Count);
    }

    public async Task<IActionResult> OnPostSignup()
    {
        var connectionString = SqlConnections.GetMySQLConnectionString();

        using var connection = new MySqlConnection(connectionString);

        string insert_query =
            @"insert into signups (email, credit_card) values (@email, @credit_card)";

        var results = await Dapper.SqlMapper.QueryAsync(
            connection,
            insert_query,
            new { email = Email, credit_card = CC }
        );

        int affected = results.ToList().Count;

        Console.WriteLine($"logged {affected} log records.");

        return Partial("_SignupThankYou");
    }
}
