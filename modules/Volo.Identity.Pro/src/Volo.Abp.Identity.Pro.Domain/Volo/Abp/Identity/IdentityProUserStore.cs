using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Features;
using Volo.Abp.Guids;
using Volo.Abp.Identity.Features;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Settings;

namespace Volo.Abp.Identity;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IdentityProUserStore), typeof(IdentityUserStore))]
public class IdentityProUserStore : IdentityUserStore
{
    protected IFeatureChecker FeatureChecker { get; }
    protected ISettingProvider SettingProvider { get; }

    public IdentityProUserStore(
        IIdentityUserRepository userRepository,
        IIdentityRoleRepository roleRepository,
        IGuidGenerator guidGenerator,
        ILogger<IdentityRoleStore> logger,
        ILookupNormalizer lookupNormalizer,
        IFeatureChecker featureChecker,
        ISettingProvider settingProvider,
        IdentityErrorDescriber describer = null)
        : base(
            userRepository,
            roleRepository,
            guidGenerator,
            logger,
            lookupNormalizer,
            describer)
    {
        FeatureChecker = featureChecker;
        SettingProvider = settingProvider;
    }

    public override async Task<bool> GetTwoFactorEnabledAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        var feature = await IdentityProTwoFactorBehaviourFeatureHelper.Get(FeatureChecker);
        if (feature == IdentityProTwoFactorBehaviour.Disabled)
        {
            return false;
        }
        if (feature == IdentityProTwoFactorBehaviour.Forced)
        {
            return true;
        }

        var setting = await IdentityProTwoFactorBehaviourSettingHelper.Get(SettingProvider);
        if (setting == IdentityProTwoFactorBehaviour.Disabled)
        {
            return false;
        }

        if (setting == IdentityProTwoFactorBehaviour.Forced)
        {
            return true;
        }

        return user.TwoFactorEnabled;
    }
}
