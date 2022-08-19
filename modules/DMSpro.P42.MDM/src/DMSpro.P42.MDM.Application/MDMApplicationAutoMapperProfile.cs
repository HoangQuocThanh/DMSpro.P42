using System;
using DMSpro.P42.MDM.Shared;
using Volo.Abp.AutoMapper;
using DMSpro.P42.MDM.Companies;
using AutoMapper;
using Volo.Abp.Identity;

namespace DMSpro.P42.MDM;

public class MDMApplicationAutoMapperProfile : Profile
{
    public MDMApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<Company, CompanyDto>();

        CreateMap<CompanyWithNavigationProperties, CompanyWithNavigationPropertiesDto>();
        CreateMap<IdentityUser, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
    }
}