using CodeMechanic.Types;
using justdoit.Models;
using NSpecifications;

namespace justdoit;

public static class TodoBusinessLogic
{
    public static List<Todo> ApplyFilters(this List<Todo> items)
    {
        bool sort_by_priority = true;
        SortDirection priority_direction = SortDirection.Descending;

        bool sort_by_due = true;
        SortDirection due_direction = SortDirection.Descending;

        return items
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
            .ToList();
    }
}