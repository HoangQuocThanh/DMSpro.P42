using System;
using AutoMapper;
using System.Linq;
using Volo.Abp.AutoMapper;

namespace Volo.Abp.Identity;

public class AbpIdentityApplicationModuleAutoMapperProfile : Profile
{
    public AbpIdentityApplicationModuleAutoMapperProfile()
    {
        CreateMap<IdentityUser, IdentityUserDto>()
            .MapExtraProperties()
            .ForMember(dest => dest.LockoutEnd, src => src.MapFrom<DateTime?>(r => r.LockoutEnd.HasValue ? r.LockoutEnd.Value.DateTime : null))
            .Ignore(x => x.IsLockedOut)
            .Ignore(x => x.SupportTwoFactor)
            .Ignore(x => x.RoleNames);

        CreateMap<IdentityRole, IdentityRoleDto>()
            .MapExtraProperties();

        CreateMap<IdentityClaimType, ClaimTypeDto>()
            .MapExtraProperties()
            .Ignore(x => x.ValueTypeAsString);

        CreateMap<IdentityUserClaim, IdentityUserClaimDto>();

        CreateMap<IdentityUserClaimDto, IdentityUserClaim>()
            .Ignore(x => x.TenantId)
            .Ignore(x => x.Id);

        CreateMap<IdentityRoleClaim, IdentityRoleClaimDto>();

        CreateMap<IdentityRoleClaimDto, IdentityRoleClaim>()
            .Ignore(x => x.TenantId)
            .Ignore(x => x.Id);

        CreateMap<CreateClaimTypeDto, IdentityClaimType>()
            .MapExtraProperties()
            .Ignore(x => x.IsStatic)
            .Ignore(x => x.Id);

        CreateMap<UpdateClaimTypeDto, IdentityClaimType>()
            .MapExtraProperties()
            .Ignore(x => x.IsStatic)
            .Ignore(x => x.Id);

        CreateMap<OrganizationUnit, OrganizationUnitDto>()
            .MapExtraProperties();

        CreateMap<OrganizationUnitRole, OrganizationUnitRoleDto>();

        CreateMap<OrganizationUnit, OrganizationUnitWithDetailsDto>()
            .MapExtraProperties()
            .Ignore(x => x.Roles);

        CreateMap<IdentityRole, OrganizationUnitRoleDto>()
            .ForMember(dest => dest.RoleId, src => src.MapFrom(r => r.Id));

        CreateMap<IdentitySecurityLog, IdentitySecurityLogDto>();

        CreateMap<IdentityRole, IdentityRoleLookupDto>();

        CreateMap<OrganizationUnit, OrganizationUnitLookupDto>();
    }
}
