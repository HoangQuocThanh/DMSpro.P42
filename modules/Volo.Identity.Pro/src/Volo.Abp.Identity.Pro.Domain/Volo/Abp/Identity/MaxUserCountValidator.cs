using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Features;
using Volo.Abp.Identity.Features;

namespace Volo.Abp.Identity;

public class MaxUserCountValidator : IUserValidator<IdentityUser>
{
    protected IFeatureChecker FeatureChecker { get; }
    protected IIdentityUserRepository UserRepository { get; }

    public MaxUserCountValidator(IFeatureChecker featureChecker, IIdentityUserRepository userRepository)
    {
        FeatureChecker = featureChecker;
        UserRepository = userRepository;
    }

    public async Task<IdentityResult> ValidateAsync(UserManager<IdentityUser> manager, IdentityUser user)
    {
        await CheckMaxUserCountAsync(user);

        return IdentityResult.Success;
    }

    protected virtual async Task CheckMaxUserCountAsync(IdentityUser user)
    {
        var maxUserCount = await FeatureChecker.GetAsync<int>(IdentityProFeature.MaxUserCount);
        if (maxUserCount <= 0)
        {
            return;
        }

        var existUser = await UserRepository.FindAsync(user.Id);
        if (existUser == null)
        {
            var currentUserCount = await UserRepository.GetCountAsync();
            if (currentUserCount >= maxUserCount)
            {
                throw new BusinessException(IdentityProErrorCodes.MaximumUserCount)
                    .WithData("MaxUserCount", maxUserCount);
            }
        }
    }
}
