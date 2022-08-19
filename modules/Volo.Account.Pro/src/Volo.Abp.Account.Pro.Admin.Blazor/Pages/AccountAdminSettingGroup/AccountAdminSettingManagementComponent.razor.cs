using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Volo.Abp.Account.ExternalProviders;
using Volo.Abp.Account.Localization;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.AspNetCore.Components.Web.Configuration;
using Volo.Abp.Features;
using Volo.Abp.Identity.Features;
using Volo.Abp.Identity.Localization;
using Volo.Abp.MultiTenancy;

namespace Volo.Abp.Account.Pro.Admin.Blazor.Pages.AccountAdminSettingGroup;

public partial class AccountAdminSettingManagementComponent
{
    [Inject]
    protected IAccountSettingsAppService AccountSettingsAppService { get; set; }

    [Inject]
    private ICurrentApplicationConfigurationCacheResetService CurrentApplicationConfigurationCacheResetService { get; set; }

    [Inject]
    protected IUiMessageService UiMessageService { get; set; }

    [Inject]
    protected IStringLocalizer<AccountResource> L { get; set; }

    [Inject]
    protected IStringLocalizer<IdentityResource> IdentityLocalizer { get; set; }

    [Inject]
    protected IFeatureChecker FeatureChecker { get; set; }

    [Inject]
    protected ICurrentTenant CurrentTenant { get; set; }

    protected AccountSettingsViewModel Settings;

    protected string SelectedTab = "AccountSettingsGeneral";

    protected Dictionary<string, bool> ExternalProviderUseHostSettings;

    protected Validations AccountCaptchaSettingsValidations;

    protected override async Task OnInitializedAsync()
    {
        Settings = new AccountSettingsViewModel
        {
            AccountSettings = await AccountSettingsAppService.GetAsync(),
            AccountRecaptchaSettings = await AccountSettingsAppService.GetRecaptchaAsync(),
            AccountExternalProviderSettings = await AccountSettingsAppService.GetExternalProviderAsync()
        };

        if (CurrentTenant.IsAvailable)
        {
            ExternalProviderUseHostSettings =
                Settings.AccountExternalProviderSettings.Settings.ToDictionary(x => x.Name, x => !x.IsValid());
        }
        
        if (await IdentityProTwoFactorBehaviourFeatureHelper.Get(FeatureChecker) == IdentityProTwoFactorBehaviour.Optional)
        {
            Settings.AccountTwoFactorSettings = await AccountSettingsAppService.GetTwoFactorAsync();
        }
    }

    public bool ShouldShowCaptchaSettings()
    {
        return Settings.AccountRecaptchaSettings != null &&
               (!CurrentTenant.IsAvailable ||
                Settings.AccountRecaptchaSettings.UseCaptchaOnLogin ||
                Settings.AccountRecaptchaSettings.UseCaptchaOnRegistration);
    }

    public bool ShouldShowExternalProviderSettings()
    {
        return (!CurrentTenant.IsAvailable && Settings.AccountExternalProviderSettings.Settings.Any())
               ||
               (CurrentTenant.IsAvailable && Settings.AccountExternalProviderSettings.Settings.Any(x => x.Enabled));
    }

    protected virtual async Task UpdateAccountSettings()
    {
        try
        {
            await AccountSettingsAppService.UpdateAsync(Settings.AccountSettings);

            await CurrentApplicationConfigurationCacheResetService.ResetAsync();

            await UiMessageService.Success(L["SavedSuccessfully"]);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected virtual async Task UpdateCaptchaSettings()
    {
        try
        {
            await AccountSettingsAppService.UpdateRecaptchaAsync(Settings.AccountRecaptchaSettings);

            await CurrentApplicationConfigurationCacheResetService.ResetAsync();

            await UiMessageService.Success(L["SavedSuccessfully"]);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected virtual async Task UpdateTwoFactorSettings()
    {
        try
        {
            await AccountSettingsAppService.UpdateTwoFactorAsync(Settings.AccountTwoFactorSettings);

            await CurrentApplicationConfigurationCacheResetService.ResetAsync();
            await UiMessageService.Success(L["SavedSuccessfully"]);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected virtual void OnExternalProviderUseHostSettingsChanged(ExternalProviderSettings providerSetting, bool value)
    {
        // use host settings
        if (value == true)
        {
            foreach (var property in providerSetting.Properties)
            {
                property.Value = null;
            }

            foreach (var property in providerSetting.SecretProperties)
            {
                property.Value = null;
            }
        }

        ExternalProviderUseHostSettings[providerSetting.Name] = value;
    }

    protected virtual async Task UpdateExternalProviderSettings()
    {
        try
        {
            var updateDto =
                    Settings.AccountExternalProviderSettings.Settings.Select(x => new UpdateExternalProviderDto
                    {
                        Enabled = x.Enabled,
                        Name = x.Name,
                        Properties = x.Properties,
                        SecretProperties = x.SecretProperties
                    }).ToList();

            await AccountSettingsAppService.UpdateExternalProviderAsync(updateDto);

            await UiMessageService.Success(L["SavedSuccessfully"]);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}

public class AccountSettingsViewModel
{
    public AccountSettingsDto AccountSettings { get; set; }

    public AccountTwoFactorSettingsDto AccountTwoFactorSettings { get; set; }

    public AccountRecaptchaSettingsDto AccountRecaptchaSettings { get; set; }

    public AccountExternalProviderSettingsDto AccountExternalProviderSettings { get; set; }
}
