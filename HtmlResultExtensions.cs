using System.Net.Mime;
using System.Text;

public static class HtmlResultExtensions
{
    /// <summary>
    /// Usage:
    /// 
    /// app.MapGet("/html", () => Results.Extensions.Html(@$"<!doctype html>
    /// <html>
    ///     <head><title>miniHTML</title></head>
    ///     <body>
    ///         <h1>Hello World</h1>
    ///         <p>The time on the server is {DateTime.Now:O}</p>
    ///     </body>
    /// </html>"));
    ///
    /// Credit: https://stackoverflow.com/questions/71711555/how-can-i-render-cshtml-from-a-mapget-route-asp-net-core
    /// </summary>
    /// <param name="resultExtensions"></param>
    /// <param name="html"></param>
    /// <returns></returns>
    public static IResult Html(this IResultExtensions resultExtensions, string html)
    {
        ArgumentNullException.ThrowIfNull(resultExtensions);

        return new HtmlResult(html);
    }
}



public class HtmlResult : IResult
{
    private readonly string _html;

    public HtmlResult(string html)
    {
        _html = html;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = MediaTypeNames.Text.Html;
        httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(_html);
        return httpContext.Response.WriteAsync(_html);
    }
}