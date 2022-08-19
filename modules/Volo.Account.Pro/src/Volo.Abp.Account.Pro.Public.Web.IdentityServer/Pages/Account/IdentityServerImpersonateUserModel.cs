using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Account.Public.Web;
using Volo.Abp.Account.Public.Web.Pages.Account;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

namespace Volo.Abp.Account.Web.Pages.Account;

[ExposeServices(typeof(ImpersonateUserModel))]
public class IdentityServerImpersonateUserModel : ImpersonateUserModel
{
    protected readonly AbpAccountIdentityServerOptions Options;

    public IdentityServerImpersonateUserModel(
        IOptions<AbpAccountOptions> accountOptions,
        IPermissionChecker permissionChecker,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IOptions<AbpAccountIdentityServerOptions> options,
        ITenantStore tenantStore)
        : base(accountOptions, permissionChecker, currentPrincipalAccessor, tenantStore)
    {
        Options = options.Value;
    }

    public async override Task<IActionResult> OnGetAsync()
    {
        if (!Request.Query.ContainsKey("access_token"))
        {
            return await base.OnGetAsync();
        }

        var authenticateResult = await HttpContext.AuthenticateAsync(Options.ImpersonationAuthenticationScheme);
        if (authenticateResult.Succeeded)
        {
            using (CurrentPrincipalAccessor.Change(authenticateResult.Principal))
            {
                if (UserId == CurrentUser.Id)
                {
                    throw new BusinessException("Volo.Account:YouCanNotImpersonateYourself");
                }

                if (AccountOptions.ImpersonationUserPermission.IsNullOrWhiteSpace() ||
                    !await PermissionChecker.IsGrantedAsync(AccountOptions.ImpersonationUserPermission))
                {
                    throw new BusinessException("Volo.Account:RequirePermissionToImpersonateUser")
                        .WithData("PermissionName", AccountOptions.ImpersonationUserPermission);
                }

                var user = await UserManager.FindByIdAsync(UserId.ToString());
                if (user != null)
                {
                    var additionalClaims = new List<Claim>();
                    if (CurrentUser.Id?.ToString() != CurrentUser.FindClaim(AbpClaimTypes.ImpersonatorUserId)?.Value)
                    {
                        additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorUserId, CurrentUser.Id.ToString()));
                        additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorUserName, CurrentUser.UserName));
                        if (CurrentTenant.IsAvailable)
                        {
                            additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorTenantId, CurrentTenant.Id.ToString()));
                            var tenantConfiguration = await TenantStore.FindAsync(CurrentTenant.Id.Value);
                            if (tenantConfiguration != null && !tenantConfiguration.Name.IsNullOrWhiteSpace())
                            {
                                additionalClaims.Add(new Claim(AbpClaimTypes.ImpersonatorTenantName, tenantConfiguration.Name));
                            }
                        }
                    }

                    try
                    {
                        await IdentityServerAuthorizeResponse.GenerateAuthorizeResponseAsync(HttpContext, user, additionalClaims.ToArray());
                    }
                    catch (Exception e)
                    {
                        Logger.LogException(e);
                        throw new BusinessException("Volo.Account:RequirePermissionToImpersonateUser")
                            .WithData("PermissionName", AccountOptions.ImpersonationUserPermission);
                    }

                    return new EmptyResult();
                }

                throw new BusinessException("Volo.Account:ThereIsNoUserWithUsernameInTheTenant")
                    .WithData("UserId", UserId);
            }
        }

        throw new BusinessException("Volo.Account:RequirePermissionToImpersonateUser")
            .WithData("PermissionName", AccountOptions.ImpersonationUserPermission);
    }
}
