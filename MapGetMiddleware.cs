using CodeMechanic.Diagnostics;
using CodeMechanic.Types;
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


        app.MapPost("/books",
                async ([FromBody] GanttPart record
                    // ,todo:  [FromServices] ITodosRepository db
                    , HttpResponse response) =>
                {
                    Console.WriteLine(record);
                    //todo: await db.Todos.AddAsync(record);
                    response.StatusCode = 200;
                    //idea: response.Headers.Location = $"books /{record.Start}/{record.End}";
                })
            .Accepts<Book>("application/json")
            .Produces<Book>(StatusCodes.Status201Created)
            .WithName("AddNewBook").WithTags("Setters");

        return app;
    }
}

public record Book
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Id { get; set; } = -1;

    public override string ToString()
    {
        return $"'{Title}', by {Author}";
    }
}

public record GanttPart
{
    public Maybe<DateTime> Start { get; set; } = Maybe<DateTime>.None;
    public Maybe<DateTime> End { get; set; } = Maybe<DateTime>.None;
}