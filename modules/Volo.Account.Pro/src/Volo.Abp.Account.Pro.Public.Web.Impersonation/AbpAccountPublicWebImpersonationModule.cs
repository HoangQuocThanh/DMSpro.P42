using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.Authentication.OpenIdConnect;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Volo.Abp.Account.Public.Web.Impersonation;

[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
    typeof(AbpAspNetCoreAuthenticationOpenIdConnectModule)
    )]
public class AbpAccountPublicWebImpersonationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, AbpAccountImpersonationJwtBearerConfigureOptions>();
        context.Services.AddSingleton<IPostConfigureOptions<OpenIdConnectOptions>, AbpAccountImpersonationOpenIdConnectConfigureOptions>();
    }
}
