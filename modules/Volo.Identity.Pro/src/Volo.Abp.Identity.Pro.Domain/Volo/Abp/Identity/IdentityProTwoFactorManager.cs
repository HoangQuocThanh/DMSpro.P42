using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Features;
using Volo.Abp.Identity.Features;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Settings;

namespace Volo.Abp.Identity;

public class IdentityProTwoFactorManager : IDomainService
{
    protected IFeatureChecker FeatureChecker { get; }

    protected ISettingProvider SettingProvider { get; }

    public IdentityProTwoFactorManager(IFeatureChecker featureChecker, ISettingProvider settingProvider)
    {
        FeatureChecker = featureChecker;
        SettingProvider = settingProvider;
    }

    public virtual async Task<bool> IsOptionalAsync()
    {
        var feature = await IdentityProTwoFactorBehaviourFeatureHelper.Get(FeatureChecker);
        if (feature == IdentityProTwoFactorBehaviour.Optional)
        {
            var setting = await IdentityProTwoFactorBehaviourSettingHelper.Get(SettingProvider);
            if (setting == IdentityProTwoFactorBehaviour.Optional)
            {
                return true;
            }
        }

        return false;
    }

    public virtual async Task<bool> IsForcedEnableAsync()
    {
        var feature = await IdentityProTwoFactorBehaviourFeatureHelper.Get(FeatureChecker);
        if (feature == IdentityProTwoFactorBehaviour.Forced)
        {
            return true;
        }

        var setting = await IdentityProTwoFactorBehaviourSettingHelper.Get(SettingProvider);
        if (setting == IdentityProTwoFactorBehaviour.Forced)
        {
            return true;
        }

        return false;
    }

    public virtual async Task<bool> IsForcedDisableAsync()
    {
        var feature = await IdentityProTwoFactorBehaviourFeatureHelper.Get(FeatureChecker);
        if (feature == IdentityProTwoFactorBehaviour.Disabled)
        {
            return true;
        }

        var setting = await IdentityProTwoFactorBehaviourSettingHelper.Get(SettingProvider);
        if (setting == IdentityProTwoFactorBehaviour.Disabled)
        {
            return true;
        }
        return false;
    }
}
