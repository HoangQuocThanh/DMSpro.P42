using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Account.ExternalProviders;
using Volo.Abp.AspNetCore.Mvc;

namespace Volo.Abp.Account;

[RemoteService(Name = AccountProPublicRemoteServiceConsts.RemoteServiceName)]
[Area(AccountProPublicRemoteServiceConsts.ModuleName)]
[Route("api/account/external-provider")]
public class AccountExternalProviderController : AbpControllerBase, IAccountExternalProviderAppService
{
    protected IAccountExternalProviderAppService AccountExternalProviderAppService { get; }

    public AccountExternalProviderController(IAccountExternalProviderAppService accountExternalProviderAppService)
    {
        AccountExternalProviderAppService = accountExternalProviderAppService;
    }

    [HttpGet]
    public virtual async Task<ExternalProviderDto> GetAllAsync()
    {
        return await AccountExternalProviderAppService.GetAllAsync();
    }

    [HttpGet]
    [Route("by-name")]
    public virtual async Task<ExternalProviderItemWithSecretDto> GetByNameAsync(GetByNameInput input)
    {
        return await AccountExternalProviderAppService.GetByNameAsync(input);
    }
}
