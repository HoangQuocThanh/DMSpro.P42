using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Volo.Abp.MultiTenancy;

namespace Volo.Abp.Account.Public.Web;

public class AbpAccountOptions
{
    /// <summary>
    /// Default value: "Windows".
    /// </summary>
    public string WindowsAuthenticationSchemeName { get; set; }

    /// <summary>
    /// Default value: "admin".
    /// </summary>
    public string TenantAdminUserName { get; set; }

    public string ImpersonationTenantPermission { get; set; }

    public string ImpersonationUserPermission { get; set; }

    public Dictionary<string, string> ExternalProviderIconMap = new Dictionary<string, string>
        {
            { "Amazon", "fa fa-amazon" },
            { "Apple", "fa fa-apple" },
            { "BattleNet", "fab fa-battle-net" },
            { "Discord", "fab fa-discord" },
            { "Dropbox", "fa fa-dropbox" },
            { "Facebook", "fa fa-facebook" },
            { "GitHub", "fa fa-github" },
            { "Google", "fa fa-google" },
            { "Instagram", "fa fa-instagram" },
            { "LinkedIn", "fa fa-linkedin" },
            { "Microsoft", "fa fa-windows" },
            { "Twitch", "fa fa-twitch" },
            { "Twitter", "fa fa-twitter" },
            { "Yandex", "fab fa-yandex-international" },
            { "Weibo", "fa fa-weibo" }
        };

    public bool IsTenantMultiDomain { get; set; }

    public Func<HttpContext, BasicTenantInfo, Task<string>> GetTenantDomain { get; set; }

    public AbpAccountOptions()
    {
        TenantAdminUserName = "admin";

        //TODO: This makes us depend on the Microsoft.AspNetCore.Server.IISIntegration package.
        WindowsAuthenticationSchemeName = "Windows"; //Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme;

        GetTenantDomain = (context, _) => Task.FromResult(context.Request.Scheme + "://" + context.Request.Host);
    }
}
