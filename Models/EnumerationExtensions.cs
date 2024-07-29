using System.Linq.Expressions;
using CodeMechanic.Diagnostics;
using CodeMechanic.Reflection;
using CodeMechanic.Types;

namespace justdoit.Models;

public static class EnumerationExtensions
{
    private static string GetName<T>(Expression<Func<T>> expression)
    {
        var member = (MemberExpression)expression.Body;
        return member.Member.Name;
    }

    
    // WARNING: Incomplete, do not use
    public static string Switch<T>(
        this T current,
        string fallback,
        Dictionary<Expression<Func<T, object>>, string> cases
    )
    {
        var pattern_strings = cases
            .Aggregate(new List<string>(), (current, kvp) =>
            {
                var property_name = MemberExtensions.GetMemberName(kvp.Key).Dump("propertyname");
                var raw_pattern = kvp.Value;

                string pattern_segment = string.IsNullOrWhiteSpace(raw_pattern) || !raw_pattern.Contains("?<")
                    ? $@"(?<{property_name}>{raw_pattern})"
                    : raw_pattern;

                current.Add(pattern_segment);
                return current;
            });
        return "test";
    }
    //     where T : Enumeration
    // {
    //     var all_enums = Enumeration.GetAll<T>();
    //
    //     if (fallback.IsEmpty())
    //         return all_enums.FirstOrDefault().Name;
    //
    // }

    /*
    public Maybe<T> Case(Action<T> if_some_value, Action if_no_value)
    {
        if (HasValue)
            if_some_value(Value);
        else
            if_no_value();
        return this;
    }
    
    
    public Maybe<U> Map<U>(Func<T, U> map_to) => HasValue
                ? Maybe.Some(map_to(Value))
                : Maybe<U>.None;
                
                */
}