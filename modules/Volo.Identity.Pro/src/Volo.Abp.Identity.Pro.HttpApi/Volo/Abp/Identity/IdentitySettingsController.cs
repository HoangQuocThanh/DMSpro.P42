using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Volo.Abp.Identity;

[RemoteService(Name = IdentityProRemoteServiceConsts.RemoteServiceName)]
[Area(IdentityProRemoteServiceConsts.ModuleName)]
[ControllerName("Settings")]
[Route("api/identity/settings")]
public class IdentitySettingsController : AbpControllerBase, IIdentitySettingsAppService
{
    protected IIdentitySettingsAppService IdentitySettingsAppService { get; }

    public IdentitySettingsController(IIdentitySettingsAppService identitySettingsAppService)
    {
        IdentitySettingsAppService = identitySettingsAppService;
    }

    [HttpGet]
    public virtual Task<IdentitySettingsDto> GetAsync()
    {
        return IdentitySettingsAppService.GetAsync();
    }

    [HttpPut]
    public virtual Task UpdateAsync(IdentitySettingsDto input)
    {
        return IdentitySettingsAppService.UpdateAsync(input);
    }

    [HttpGet]
    [Route("ldap")]
    public virtual async Task<IdentityLdapSettingsDto> GetLdapAsync()
    {
        return await IdentitySettingsAppService.GetLdapAsync();
    }

    [HttpPut]
    [Route("ldap")]
    public virtual async Task UpdateLdapAsync(IdentityLdapSettingsDto input)
    {
        await IdentitySettingsAppService.UpdateLdapAsync(input);
    }

    [HttpGet]
    [Route("oauth")]
    public Task<IdentityOAuthSettingsDto> GetOAuthAsync()
    {
        return IdentitySettingsAppService.GetOAuthAsync();
    }

    [HttpPut]
    [Route("oauth")]
    public Task UpdateOAuthAsync(IdentityOAuthSettingsDto input)
    {
        return IdentitySettingsAppService.UpdateOAuthAsync(input);
    }
}
