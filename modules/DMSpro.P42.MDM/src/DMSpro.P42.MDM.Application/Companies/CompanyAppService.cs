using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using DMSpro.P42.MDM.Permissions;
using DMSpro.P42.MDM.Companies;

namespace DMSpro.P42.MDM.Companies
{

    [Authorize(MDMPermissions.Companies.Default)]
    public class CompaniesAppService : ApplicationService, ICompaniesAppService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly CompanyManager _companyManager;

        public CompaniesAppService(ICompanyRepository companyRepository, CompanyManager companyManager)
        {
            _companyRepository = companyRepository;
            _companyManager = companyManager;
        }

        public virtual async Task<PagedResultDto<CompanyDto>> GetListAsync(GetCompaniesInput input)
        {
            var totalCount = await _companyRepository.GetCountAsync(input.FilterText, input.Code, input.Name, input.Address1);
            var items = await _companyRepository.GetListAsync(input.FilterText, input.Code, input.Name, input.Address1, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<CompanyDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Company>, List<CompanyDto>>(items)
            };
        }

        public virtual async Task<CompanyDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Company, CompanyDto>(await _companyRepository.GetAsync(id));
        }

        [Authorize(MDMPermissions.Companies.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _companyRepository.DeleteAsync(id);
        }

        [Authorize(MDMPermissions.Companies.Create)]
        public virtual async Task<CompanyDto> CreateAsync(CompanyCreateDto input)
        {

            var company = await _companyManager.CreateAsync(
            input.Code, input.Name, input.Address1
            );

            return ObjectMapper.Map<Company, CompanyDto>(company);
        }

        [Authorize(MDMPermissions.Companies.Edit)]
        public virtual async Task<CompanyDto> UpdateAsync(Guid id, CompanyUpdateDto input)
        {

            var company = await _companyManager.UpdateAsync(
            id,
            input.Code, input.Name, input.Address1, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Company, CompanyDto>(company);
        }
    }
}