using CodeMechanic.Diagnostics;
using CodeMechanic.Types;
using justdoit.Models;
using NSpecifications;

namespace justdoit;

public static class TodoBusinessLogic
{
    public record TodoFilters
    {
    }

    public record TodoSorts
    {
    }


    public static IEnumerable<T> ForEachMutate<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);

        return source;
    }

    public static List<Todo> AutoSchedule(this List<Todo> items)
    {
        var now = DateTime.Now;

        // debug
        var zero_dates = items.Where(x => x.due.Equals(DateTime.MinValue)).ToList();
        zero_dates.Count.Dump("total Null dates");

        var full_week = items
            .OrderByDescending(t => t.due.Month)
            // re-assign null dates to a random day in the future, for now.  TODO: once you have sse working, remove this.
            .ForEachMutate(t =>
            {
                if (t.due.Equals(DateTime.MinValue))
                {
                    t.due = now.AddDays(7);
                }
            })
            .ToList();

        return full_week;
    }

    public static List<Todo> ApplyFilters(this List<Todo> items, string query)
    {
        bool sort_by_priority = true;
        SortDirection priority_direction = SortDirection.Descending;

        bool sort_by_due = true;
        SortDirection due_direction = SortDirection.Descending;

        var sorted_todos = items
            .If(sort_by_priority, todos =>
                Equals(due_direction, SortDirection.Descending)
                    ? todos.OrderByDescending(todo => todo.priority)
                    : todos.OrderBy(todo => todo.priority)
            )
            .If(sort_by_due, todos =>
                Equals(due_direction, SortDirection.Descending)
                    ? todos.OrderByDescending(todo => todo.due)
                    : todos.OrderBy(todo => todo.due)
            )
            .If(query.NotEmpty(), todos => todos
                .Where(t => t.content
                    .Contains(query, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        return sorted_todos;
    }
}