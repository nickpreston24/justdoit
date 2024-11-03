using CodeMechanic.Types;

namespace justdoit;

public static class BashExts
{
    // TODO: Put this functionality in DotEnv.cs.
    public static async Task<string> Echo(this string variable_name)
    {
        if (!variable_name.IsEmpty())
            return await $"echo {variable_name}".Bash();
        return string.Empty;
    }
}
