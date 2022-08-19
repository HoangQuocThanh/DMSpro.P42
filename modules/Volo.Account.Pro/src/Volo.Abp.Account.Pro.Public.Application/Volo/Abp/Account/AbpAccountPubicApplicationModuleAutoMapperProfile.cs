using System;
using AutoMapper;
using Volo.Abp.Account.ExternalProviders;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;

namespace Volo.Abp.Account;

public class AbpAccountPubicApplicationModuleAutoMapperProfile : Profile
{
    public AbpAccountPubicApplicationModuleAutoMapperProfile()
    {
        CreateMap<ExternalProviderSettings, ExternalProviderItemDto>(MemberList.Destination);
        CreateMap<ExternalProviderSettings, ExternalProviderItemWithSecretDto>(MemberList.Destination)
            .ForMember(d => d.Success, opt => opt.MapFrom(x => !x.Name.IsNullOrWhiteSpace()));

        CreateMap<IdentityUser, ProfileDto>()
            .ForMember(dest => dest.HasPassword,
                op => op.MapFrom(src => src.PasswordHash != null))
            .MapExtraProperties();

        CreateMap<IdentityUser, IdentityUserDto>()
            .MapExtraProperties()
            .ForMember(dest => dest.LockoutEnd, src => src.MapFrom<DateTime?>(r => r.LockoutEnd.HasValue ? r.LockoutEnd.Value.DateTime : null))
            .Ignore(x => x.IsLockedOut)
            .Ignore(x => x.SupportTwoFactor)
            .Ignore(x => x.RoleNames);

        CreateMap<IdentitySecurityLog, IdentitySecurityLogDto>();
    }
}
