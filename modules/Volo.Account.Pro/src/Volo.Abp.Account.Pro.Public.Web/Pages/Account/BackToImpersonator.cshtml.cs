using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Security.Claims;

namespace Volo.Abp.Account.Public.Web.Pages.Account;

[Authorize]
[IgnoreAntiforgeryToken]
public class BackToImpersonatorModel : AccountPageModel
{
    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; }

    public BackToImpersonatorModel(ICurrentPrincipalAccessor currentPrincipalAccessor)
    {
        CurrentPrincipalAccessor = currentPrincipalAccessor;
    }

    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(NotFound());
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        var impersonatorTenantId = CurrentPrincipalAccessor.Principal.FindImpersonatorTenantId();
        var impersonatorUserId = CurrentPrincipalAccessor.Principal.FindImpersonatorUserId();

        if (impersonatorTenantId == null && impersonatorUserId == null || impersonatorUserId == null)
        {
            return RedirectSafely("~/");
        }

        using (CurrentTenant.Change(impersonatorTenantId))
        {
            var user = await UserManager.GetByIdAsync(impersonatorUserId.Value);
            var isPersistent = (await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme))?.Properties?.IsPersistent ?? false;
            await SignInManager.SignOutAsync();

            await SignInManager.SignInAsync(user, new AuthenticationProperties
            {
                IsPersistent = isPersistent
            });

            return Redirect("~/");
        }
    }
}
