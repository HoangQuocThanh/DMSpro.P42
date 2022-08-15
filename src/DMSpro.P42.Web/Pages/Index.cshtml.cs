using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace DMSpro.P42.Web.Pages;

public class IndexModel : P42PageModel
{
    public void OnGet()
    {

    }

    public async Task OnPostLoginAsync()
    {
        await HttpContext.ChallengeAsync("oidc");
    }
}
