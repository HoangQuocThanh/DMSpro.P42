using Volo.Abp.AutoMapper;
using DMSpro.P42.MDM.Companies;
using AutoMapper;

namespace DMSpro.P42.MDM.Web;

public class MDMWebAutoMapperProfile : Profile
{
    public MDMWebAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<CompanyDto, CompanyUpdateDto>();

        CreateMap<CompanyDto, CompanyUpdateDto>().Ignore(x => x.IdentityUserIds);
    }
}