using Coravel.Queuing.Interfaces;
// using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace justdoit.Pages.Admin;

public class LongRunningInvocable : PageModel
{
    private readonly IQueue invocables;

    public LongRunningInvocable(IQueue queue)
    {
        this.invocables = queue;
    }

    // public void Task OnGetCalculation()
    // {
    //
    //     // return Ok();
    // }
}
