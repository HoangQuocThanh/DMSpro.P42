using Volo.Abp.Identity.Features;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace Volo.Abp.Identity;

public class AbpIdentityProSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(
           new SettingDefinition(IdentityProSettingNames.TwoFactor.Behaviour,
                IdentityProTwoFactorBehaviour.Optional.ToString(),
                L("DisplayName:Abp.Identity.TwoFactorBehaviour"),
                L("Description:Abp.Identity.TwoFactorBehaviour"),
                isVisibleToClients: true),

            new SettingDefinition(IdentityProSettingNames.TwoFactor.UsersCanChange,
                true.ToString(),
                L("DisplayName:Abp.Identity.UsersCanChange"),
                L("Description:Abp.Identity.UsersCanChange"),
                isVisibleToClients: true),

           new SettingDefinition(IdentityProSettingNames.EnableLdapLogin, "false",
               L("DisplayName:Abp.Identity.EnableLdapLogin"),
               L("Description:Abp.Identity.EnableLdapLogin"),
               isVisibleToClients: true),
           
           new SettingDefinition(IdentityProSettingNames.EnableOAuthLogin, "false",
                L("DisplayName:Abp.Identity.EnableOAuthLogin"),
                L("Description:Abp.Identity.EnableOAuthLogin"),
                isVisibleToClients: true),
           
           new SettingDefinition(IdentityProSettingNames.OAuthLogin.Authority, 
               null,
               L("DisplayName:Abp.Identity.Authority"),
               L("Description:Abp.Identity.Authority"), 
               true),
           
           new SettingDefinition(IdentityProSettingNames.OAuthLogin.ClientId, 
               null,
               L("DisplayName:Abp.Identity.ClientId"),
               L("Description:Abp.Identity.ClientId"), 
               true),
           
           new SettingDefinition(IdentityProSettingNames.OAuthLogin.ClientSecret, 
               null,
               L("DisplayName:Abp.Identity.ClientSecret"),
               L("Description:Abp.Identity.ClientSecret"), 
               true),
           
           new SettingDefinition(IdentityProSettingNames.OAuthLogin.Scope, 
               null,
               L("DisplayName:Abp.Identity.Scope"),
               L("Description:Abp.Identity.Scope"), 
               true),
           
           new SettingDefinition(IdentityProSettingNames.OAuthLogin.RequireHttpsMetadata, 
               "false",
               L("DisplayName:Abp.Identity.RequireHttpsMetadata"),
               L("Description:Abp.Identity.RequireHttpsMetadata"), 
               true)
        );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<IdentityResource>(name);
    }
}
