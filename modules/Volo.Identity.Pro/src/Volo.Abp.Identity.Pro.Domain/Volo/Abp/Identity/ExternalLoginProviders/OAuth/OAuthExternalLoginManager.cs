using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity.Settings;

namespace Volo.Abp.Identity.ExternalLoginProviders.OAuth;

public class OAuthExternalLoginManager : ITransientDependency
{
    public const string HttpClientName = "OAuthExternalLoginManager";
    
    public ILogger<OAuthExternalLoginManager> Logger { get; set; }
    
    protected IOAuthSettingProvider OAuthSettingProvider { get; }
    protected IHttpClientFactory HttpClientFactory { get; }

    public OAuthExternalLoginManager(
        IOAuthSettingProvider oAuthSettingProvider,
        IHttpClientFactory httpClientFactory)
    {
        OAuthSettingProvider = oAuthSettingProvider;
        HttpClientFactory = httpClientFactory;
        Logger = NullLogger<OAuthExternalLoginManager>.Instance;
    }

    public virtual async Task<bool> AuthenticateAsync(string userName, string password)
    {
        try
        {
            await GetAccessTokenAsync(userName, password);
            return true;
        }
        catch (AbpException ex)
        {
            Logger.LogException(ex);
            return false;
        }
    }

    public virtual async Task<IEnumerable<Claim>> GetUserInfoAsync(string userName, string password)
    {
        using (var httpClient = HttpClientFactory.CreateClient(HttpClientName))
        {
            var token = await GetAccessTokenAsync(userName, password);
            var discoveryResponse = await GetDiscoveryResponseAsync();
            
            var userinfoResponse = await httpClient.GetUserInfoAsync(
                new UserInfoRequest
                {
                    Token = token, 
                    Address = discoveryResponse.UserInfoEndpoint
                });

            if (userinfoResponse.IsError)
            {
                throw userinfoResponse.Exception ?? new AbpException("Get user info error: " + userinfoResponse.Raw);
            }

            return userinfoResponse.Claims;
        }
    }
    
    protected virtual async Task<string> GetAccessTokenAsync(string userName, string password)
    {
        using (var httpClient = HttpClientFactory.CreateClient(HttpClientName))
        {
            var discoveryResponse = await GetDiscoveryResponseAsync();
            
            var request = new PasswordTokenRequest
            {
                Address = discoveryResponse.TokenEndpoint,
                ClientId = await OAuthSettingProvider.GetClientIdAsync(),
                ClientSecret = await OAuthSettingProvider.GetClientSecretAsync(),
                Scope = await OAuthSettingProvider.GetScopeAsync(),
                UserName = userName,
                Password = password
            };

            var tokenResponse = await httpClient.RequestPasswordTokenAsync(request);
            if (tokenResponse.IsError)
            {
                throw tokenResponse.Exception ?? new AbpException("Get access token error: " + tokenResponse.Raw);
            }
            
            return tokenResponse.AccessToken;
        }
    }

    protected virtual async Task<DiscoveryDocumentResponse> GetDiscoveryResponseAsync()
    {
        using (var httpClient = HttpClientFactory.CreateClient(HttpClientName))
        {
            var request = new DiscoveryDocumentRequest
            {
                Address = await OAuthSettingProvider.GetAuthorityAsync(),
                Policy = new DiscoveryPolicy
                {
                    RequireHttps = await OAuthSettingProvider.GetRequireHttpsMetadataAsync()
                }
            };
            
            var discoveryResponse = await httpClient.GetDiscoveryDocumentAsync(request);
            if (discoveryResponse.IsError)
            {
                throw discoveryResponse.Exception ?? new AbpException("Get discovery error: " + discoveryResponse.Raw);
            }

            return discoveryResponse;
        }
    }
}