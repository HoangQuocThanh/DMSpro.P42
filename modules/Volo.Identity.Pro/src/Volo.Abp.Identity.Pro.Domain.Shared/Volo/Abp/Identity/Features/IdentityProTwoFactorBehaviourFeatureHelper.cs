using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp.Features;

namespace Volo.Abp.Identity.Features;

public static class IdentityProTwoFactorBehaviourFeatureHelper
{
    public static async Task<IdentityProTwoFactorBehaviour> Get([NotNull] IFeatureChecker featureChecker)
    {
        Check.NotNull(featureChecker, nameof(featureChecker));

        var value = await featureChecker.GetOrNullAsync(IdentityProFeature.TwoFactor);
        if (value.IsNullOrWhiteSpace() || !Enum.TryParse<IdentityProTwoFactorBehaviour>(value, out var behaviour))
        {
            throw new AbpException($"{IdentityProFeature.TwoFactor} feature value is invalid");
        }

        return behaviour;
    }
}
