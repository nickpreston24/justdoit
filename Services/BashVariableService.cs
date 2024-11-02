namespace justdoit;

public class BashVariableService
{
    public async Task EchoVar(string variable_name = "$FOO")
    {
        await $"echo {variable_name}".Bash();
    }
}
