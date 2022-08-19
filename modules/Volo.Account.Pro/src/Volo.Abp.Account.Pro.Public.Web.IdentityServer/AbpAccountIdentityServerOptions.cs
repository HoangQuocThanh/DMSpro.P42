using System;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Volo.Abp.MultiTenancy;

namespace Volo.Abp.Account.Web;

public class AbpAccountIdentityServerOptions
{
    public string ImpersonationAuthenticationScheme { get; set; }

    public bool IsTenantMultiDomain { get; set; }

    public Func<HttpContext, ExtensionGrantValidationContext, BasicTenantInfo, Task<string>> GetTenantDomain { get; set; }

    public AbpAccountIdentityServerOptions()
    {
        ImpersonationAuthenticationScheme = "Bearer";

        GetTenantDomain = (context, _, _) => Task.FromResult(context.Request.Scheme + "://" + context.Request.Host);
    }
}
