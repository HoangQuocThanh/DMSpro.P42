using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace Volo.Abp.Account.Public.Web.Impersonation;

public class AbpAccountImpersonationOpenIdConnectConfigureOptions : IPostConfigureOptions<OpenIdConnectOptions>
{
    public void PostConfigure(string name, OpenIdConnectOptions options)
    {
        options.Events ??= new OpenIdConnectEvents();

        var previousOnRedirectToIdentityProvider = options.Events.OnRedirectToIdentityProvider;
        options.Events.OnRedirectToIdentityProvider = async redirectContext =>
        {
            if (redirectContext.Properties.Items.ContainsKey(AbpAccountImpersonationConsts.Impersonation))
            {
                redirectContext.ProtocolMessage.Parameters.Add(AbpAccountImpersonationConsts.AccessToken, await redirectContext.HttpContext.GetTokenAsync(AbpAccountImpersonationConsts.AccessToken));
                if (redirectContext.Properties.Items.ContainsKey(AbpAccountImpersonationConsts.UserId))
                {
                    redirectContext.ProtocolMessage.IssuerAddress = options.Authority.EnsureEndsWith('/') + AbpAccountImpersonationConsts.ImpersonateUserEndpoint;
                    redirectContext.ProtocolMessage.Parameters.Add(AbpAccountImpersonationConsts.UserId, redirectContext.Properties.Items[AbpAccountImpersonationConsts.UserId]);
                }
                else if (redirectContext.Properties.Items.ContainsKey(AbpAccountImpersonationConsts.TenantId))
                {
                    redirectContext.ProtocolMessage.IssuerAddress = options.Authority.EnsureEndsWith('/') + AbpAccountImpersonationConsts.ImpersonateTenantEndpoint;
                    redirectContext.ProtocolMessage.Parameters.Add(AbpAccountImpersonationConsts.TenantId, redirectContext.Properties.Items[AbpAccountImpersonationConsts.TenantId]);
                }
            }
            else if (redirectContext.Properties.Items.ContainsKey(AbpAccountImpersonationConsts.BackToImpersonator))
            {
                redirectContext.ProtocolMessage.Parameters.Add(AbpAccountImpersonationConsts.AccessToken, await redirectContext.HttpContext.GetTokenAsync(AbpAccountImpersonationConsts.AccessToken));
                redirectContext.ProtocolMessage.IssuerAddress = options.Authority.EnsureEndsWith('/') + AbpAccountImpersonationConsts.BackToImpersonatorEndpoint;
            }

            if (previousOnRedirectToIdentityProvider != null)
            {
                await previousOnRedirectToIdentityProvider(redirectContext);
            }
        };
    }
}
