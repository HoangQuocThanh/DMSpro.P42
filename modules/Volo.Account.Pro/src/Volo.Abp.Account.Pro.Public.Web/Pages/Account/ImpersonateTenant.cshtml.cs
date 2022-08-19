using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;

namespace Volo.Abp.Account.Public.Web.Pages.Account;

[Authorize]
[IgnoreAntiforgeryToken]
public class ImpersonateTenantModel : AccountPageModel
{
    [BindProperty(SupportsGet = true)]
    [Required]
    public Guid TenantId { get; set; }

    protected AbpAccountOptions AccountOptions { get; }
    protected IPermissionChecker PermissionChecker { get; }
    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; }

    public ImpersonateTenantModel(
        IOptions<AbpAccountOptions> accountOptions,
        IPermissionChecker permissionChecker,
        ICurrentPrincipalAccessor currentPrincipalAccessor)
    {
        AccountOptions = accountOptions.Value;
        PermissionChecker = permissionChecker;
        CurrentPrincipalAccessor = currentPrincipalAccessor;
    }

    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(NotFound());
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        if (CurrentUser.FindImpersonatorUserId() != null)
        {
            throw new BusinessException("Volo.Account:NestedImpersonationIsNotAllowed");
        }

        if (CurrentTenant.Id != null)
        {
            throw new BusinessException("Volo.Account:ImpersonateTenantOnlyAvailableForHost");
        }

        if (AccountOptions.ImpersonationTenantPermission.IsNullOrWhiteSpace() ||
            !await PermissionChecker.IsGrantedAsync(AccountOptions.ImpersonationTenantPermission))
        {
            throw new BusinessException("Volo.Account:RequirePermissionToImpersonateTenant")
                .WithData("PermissionName", AccountOptions.ImpersonationTenantPermission);
        }

        var currentUserId = CurrentUser.Id;
        using (CurrentTenant.Change(TenantId))
        {
            var adminUser = await UserManager.FindByNameAsync(AccountOptions.TenantAdminUserName);
            if (adminUser != null)
            {
                var isPersistent = (await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme))?.Properties?.IsPersistent ?? false;
                await SignInManager.SignOutAsync();

                var additionalClaims = new List<Claim>
                    {
                        new Claim(AbpClaimTypes.ImpersonatorUserId, currentUserId.ToString()),
                        new Claim(AbpClaimTypes.ImpersonatorUserName, CurrentUser.UserName)
                    };

                await SignInManager.SignInWithClaimsAsync(adminUser, new AuthenticationProperties
                {
                    IsPersistent = isPersistent
                }, additionalClaims);

                //save security log to admin user.
                var userPrincipal = await SignInManager.CreateUserPrincipalAsync(adminUser);
                userPrincipal.Identities.First().AddClaims(additionalClaims);
                using (CurrentPrincipalAccessor.Change(userPrincipal))
                {
                    await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                    {
                        Identity = IdentitySecurityLogIdentityConsts.Identity,
                        Action = "ImpersonateUser"
                    });
                }

                return Redirect("~/");
            }

            throw new BusinessException("Volo.Account:ThereIsNoUserWithUserName")
                .WithData("UserName", AccountOptions.TenantAdminUserName);
        }
    }
}
