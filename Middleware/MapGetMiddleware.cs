using justdoit.Models;
using Microsoft.AspNetCore.Mvc;

public static class MapGetMiddleware
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        app.MapGet("/html", () => Microsoft.AspNetCore.Http.Results.Extensions.Html(
            @$"<!doctype html>
                <html>
                    <head><title>miniHTML</title></head>
                    <body>
                        <h1 class='text-primary border-2 border-red-500'>Hello World</h1>
                        <p>The time on the server is {DateTime.Now:O}</p>
                    </body>
                </html>"));

        app.MapPost("/books",
                async ([FromBody] Book record
                    // , [FromServices] BooksDB db
                    , HttpResponse response) =>
                {
                    Console.WriteLine(record);
                    // db.Books.Add(record);
                    // await db.SaveChangesAsync();
                    response.StatusCode = 200;
                    response.Headers.Location = $"books /{record.Id}";
                })
            .Accepts<Book>("application/json")
            .Produces<Book>(StatusCodes.Status201Created)
            .WithName("AddNewBook").WithTags("Setters");

        app.MapPost("/gantt",
                async ([FromBody] GanttRequest record
                    // ,todo:  [FromServices] ITodosRepository db
                    , HttpResponse response) =>
                {
                    // record.Dump(nameof(record));
                    Console.WriteLine(record);
                    //todo: await db.Todos.AddAsync(record);
                    response.StatusCode = 200;
                    //idea: response.Headers.Location = $"books /{record.Start}/{record.End}";
                })
            // .Accepts<GanttRequest>("application/json")
            // .Produces<GanttRequest>(StatusCodes.Status201Created)
            // .WithName("GenerateGantt")
            // .WithTags("Setters")
            ;

        return app;
    }
}