using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp.Identity.Features;
using Volo.Abp.Settings;

namespace Volo.Abp.Identity.Settings;

public static class IdentityProTwoFactorBehaviourSettingHelper
{
    public static async Task<IdentityProTwoFactorBehaviour> Get([NotNull] ISettingProvider settingProvider)
    {
        Check.NotNull(settingProvider, nameof(settingProvider));

        var value = await settingProvider.GetOrNullAsync(IdentityProSettingNames.TwoFactor.Behaviour);
        if (value.IsNullOrWhiteSpace() || !Enum.TryParse<IdentityProTwoFactorBehaviour>(value, out var behaviour))
        {
            throw new AbpException($"{IdentityProSettingNames.TwoFactor.Behaviour} setting value is invalid");
        }

        return behaviour;
    }
}
