// using CodeMechanic.RegularExpressions;  NOTE: importing this dll breaks microsoft.  I don't know why.  I think it's because MS somehow interprets 'regularexpressions' as a duplicate dll.

using System.Reflection;
using System.Text.RegularExpressions;

namespace justdoit;

public class TodoPriorityRegex : RegexEnumBase
{
    public static TodoPriorityRegex Basic = new TodoPriorityRegex(
        1,
        nameof(Basic),
        @"(?<raw_text>(priority\s*|p)(?<Value>[1-4]))",
        "https://regex101.com/r/twefSL/1"
    ); // return that part to autozone tomorrow p1

    protected TodoPriorityRegex(int id, string name, string pattern, string uri = "")
        : base(id, name, pattern, uri) { }
}

public class RegexEnumBase : Enumeration
{
    protected RegexEnumBase(int id, string name, string pattern, string uri = "")
        : base(id, name)
    {
        Pattern = pattern;
        CompiledRegex = new System.Text.RegularExpressions.Regex(
            pattern,
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );
        this.uri = uri;
    }

    public string uri { get; set; } = string.Empty;

    public Regex CompiledRegex { get; set; }
    public string Pattern { get; set; }
}

public abstract class Enumeration : IComparable
{
    public string Name { get; private set; } = string.Empty;

    public int Id { get; private set; }

    protected Enumeration(int id, string name) => (Id, Name) = (id, name);

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>()
        where T : Enumeration =>
        typeof(T)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();

    public override bool Equals(object obj)
    {
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        var typeMatches = GetType().Equals(obj.GetType());
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);

    // Other utility methods ...


    // Mine
    public static implicit operator Enumeration(string name)
    {
        var enumeration = GetAll<Enumeration>()
            .SingleOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return enumeration;
    }

    // From Jimmy B.  / Reuben Bond
    // Credit: https://github.com/dotnet-architecture/eShopOnContainers/blob/dev/src/Services/Ordering/Ordering.Domain/SeedWork/Enumeration.cs

    public override int GetHashCode() => Id.GetHashCode();

    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
    {
        var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
        return absoluteDifference;
    }

    public static T FromValue<T>(int value)
        where T : Enumeration
    {
        var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
        return matchingItem;
    }

    public static T FromDisplayName<T>(string displayName)
        where T : Enumeration
    {
        var matchingItem = Parse<T, string>(
            displayName,
            "display name",
            item => item.Name == displayName
        );
        return matchingItem;
    }

    private static T Parse<T, K>(K value, string description, Func<T, bool> predicate)
        where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

        if (matchingItem == null)
            throw new InvalidOperationException(
                $"'{value}' is not a valid {description} in {typeof(T)}"
            );

        return matchingItem;
    }
}
