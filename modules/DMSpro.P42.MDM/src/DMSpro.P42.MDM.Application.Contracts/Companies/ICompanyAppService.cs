using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DMSpro.P42.MDM.Companies
{
    public interface ICompaniesAppService : IApplicationService
    {
        Task<PagedResultDto<CompanyDto>> GetListAsync(GetCompaniesInput input);

        Task<CompanyDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<CompanyDto> CreateAsync(CompanyCreateDto input);

        Task<CompanyDto> UpdateAsync(Guid id, CompanyUpdateDto input);
    }
}