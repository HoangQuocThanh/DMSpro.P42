// This file is automatically generated by ABP framework to use MVC Controllers from CSharp
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Modeling;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client.ClientProxying;
using Volo.Abp.Identity;

// ReSharper disable once CheckNamespace
namespace Volo.Abp.Identity.ClientProxies;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IIdentitySecurityLogAppService), typeof(IdentitySecurityLogClientProxy))]
public partial class IdentitySecurityLogClientProxy : ClientProxyBase<IIdentitySecurityLogAppService>, IIdentitySecurityLogAppService
{
    public virtual async Task<PagedResultDto<IdentitySecurityLogDto>> GetListAsync(GetIdentitySecurityLogListInput input)
    {
        return await RequestAsync<PagedResultDto<IdentitySecurityLogDto>>(nameof(GetListAsync), new ClientProxyRequestTypeValue
        {
            { typeof(GetIdentitySecurityLogListInput), input }
        });
    }

    public virtual async Task<IdentitySecurityLogDto> GetAsync(Guid id)
    {
        return await RequestAsync<IdentitySecurityLogDto>(nameof(GetAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }

    public virtual async Task<PagedResultDto<IdentitySecurityLogDto>> GetMyListAsync(GetIdentitySecurityLogListInput input)
    {
        return await RequestAsync<PagedResultDto<IdentitySecurityLogDto>>(nameof(GetMyListAsync), new ClientProxyRequestTypeValue
        {
            { typeof(GetIdentitySecurityLogListInput), input }
        });
    }

    public virtual async Task<IdentitySecurityLogDto> GetMyAsync(Guid id)
    {
        return await RequestAsync<IdentitySecurityLogDto>(nameof(GetMyAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }
}
