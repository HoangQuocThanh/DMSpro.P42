using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Authentication;

namespace Volo.Abp.Account.Public.Web.Impersonation;

public abstract class AbpAccountImpersonationChallengeAccountController : ChallengeAccountController
{
    [Authorize]
    [IgnoreAntiforgeryToken]
    public virtual Task<IActionResult> ImpersonateTenantAsync(Guid tenantId)
    {
        if (CurrentUser.IsAuthenticated)
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string>()
                {
                    { AbpAccountImpersonationConsts.Impersonation, AbpAccountImpersonationConsts.Impersonation },
                    { AbpAccountImpersonationConsts.TenantId, tenantId.ToString() }
                })
            {
                RedirectUri = "/"
            };

            return Task.FromResult<IActionResult>(Challenge(properties, ChallengeAuthenticationSchemas));
        }

        return Task.FromResult<IActionResult>(Unauthorized());
    }

    [Authorize]
    [IgnoreAntiforgeryToken]
    public virtual Task<IActionResult> ImpersonateUserAsync(Guid userId)
    {
        if (CurrentUser.IsAuthenticated)
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string>()
                {
                    { AbpAccountImpersonationConsts.Impersonation, AbpAccountImpersonationConsts.Impersonation },
                    { AbpAccountImpersonationConsts.UserId, userId.ToString() }
                })
            {
                RedirectUri = "/"
            };

            return Task.FromResult<IActionResult>(Challenge(properties, ChallengeAuthenticationSchemas));
        }

        return Task.FromResult<IActionResult>(Unauthorized());
    }

    [Authorize]
    [IgnoreAntiforgeryToken]
    public virtual Task<IActionResult> BackToImpersonatorAsync()
    {
        if (CurrentUser.IsAuthenticated)
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string>()
                {
                    { AbpAccountImpersonationConsts.Impersonation, AbpAccountImpersonationConsts.Impersonation }
                })
            {
                RedirectUri = "/"
            };

            return Task.FromResult<IActionResult>(Challenge(properties, ChallengeAuthenticationSchemas));
        }

        return Task.FromResult<IActionResult>(Unauthorized());
    }
}
