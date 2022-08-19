﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Volo.Abp.Features;
using Volo.Abp.Identity.Features;
using Xunit;


namespace Volo.Abp.Identity;

public class MaxUserCountValidator_Tests : AbpIdentityDomainTestBase
{

    private readonly IIdentityUserRepository _userRepository;
    private readonly IdentityUserManager _identityUserManager;

    public MaxUserCountValidator_Tests()
    {
        _userRepository = GetRequiredService<IIdentityUserRepository>();
        _identityUserManager = GetRequiredService<IdentityUserManager>();
    }

    protected override void AfterAddApplication(IServiceCollection services)
    {
        var featureChecker = Substitute.For<IFeatureChecker>();
        featureChecker.GetOrNullAsync(IdentityProFeature.MaxUserCount).Returns("100");
        services.AddSingleton(featureChecker);
    }

    [Fact]
    public async Task MaxUserCount_Test()
    {
        var currCount =  await _userRepository.GetCountAsync();
        var id = Guid.NewGuid();
        for (var i = 0; i < 100 - currCount; i++)
        {
            id = Guid.NewGuid();
            await _identityUserManager.CreateAsync(new IdentityUser(id, id.ToString("N"), $"{id:N}@abp.io"));
        }

        await Assert.ThrowsAnyAsync<BusinessException>(async () =>
            await _identityUserManager.CreateAsync(new IdentityUser(Guid.NewGuid(), Guid.NewGuid().ToString("N"),
                $"{Guid.NewGuid():N}@abp.io")));

        var admin = await _userRepository.GetAsync(id);
        admin.Surname = "admin surname";
        await _identityUserManager.UpdateAsync(admin);
    }
}
