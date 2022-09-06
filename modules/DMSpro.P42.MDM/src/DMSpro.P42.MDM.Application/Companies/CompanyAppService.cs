using DMSpro.P42.MDM.Shared;
using Volo.Abp.Identity;
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
using Volo.Abp.ObjectMapping;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;

namespace DMSpro.P42.MDM.Companies
{

    [Authorize(MDMPermissions.Companies.Default)]
    public class CompaniesAppService : ApplicationService, ICompaniesAppService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly CompanyManager _companyManager;
        private readonly IRepository<IdentityUser, Guid> _identityUserRepository;

        public CompaniesAppService(ICompanyRepository companyRepository, CompanyManager companyManager, IRepository<IdentityUser, Guid> identityUserRepository)
        {
            _companyRepository = companyRepository;
            _companyManager = companyManager; _identityUserRepository = identityUserRepository;
        }

        /*ThanhHQ*/
        public async Task<LoadResult> GetAllAsync(DataSourceLoadOptionsBase loadOptions)
        {
            //var result = await Task.FromResult(DataSourceLoader.Load(await _companyRepository.GetQueryableAsync(), loadOptions));
            //result.data = ObjectMapper.Map<IEnumerable<Company>, IEnumerable<CompanyDto>>(result.data.Cast<Company>());

            return DataSourceLoader.Load(await _companyRepository.GetQueryableAsync(), loadOptions); ;
        }

        public virtual async Task<PagedResultDto<CompanyWithNavigationPropertiesDto>> GetListAsync(GetCompaniesInput input)
        {
            var totalCount = await _companyRepository.GetCountAsync(input.FilterText, input.Code, input.Name, input.Address1, input.IdentityUserId);
            var items = await _companyRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Code, input.Name, input.Address1, input.IdentityUserId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<CompanyWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<CompanyWithNavigationProperties>, List<CompanyWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<CompanyWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<CompanyWithNavigationProperties, CompanyWithNavigationPropertiesDto>
                (await _companyRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<CompanyDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Company, CompanyDto>(await _companyRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetIdentityUserLookupAsync(LookupRequestDto input)
        {
            var query = (await _identityUserRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<IdentityUser>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<IdentityUser>, List<LookupDto<Guid>>>(lookupData)
            };
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
            input.IdentityUserIds, input.Code, input.Name, input.Address1
            );

            return ObjectMapper.Map<Company, CompanyDto>(company);
        }

        [Authorize(MDMPermissions.Companies.Edit)]
        public virtual async Task<CompanyDto> UpdateAsync(Guid id, CompanyUpdateDto input)
        {

            var company = await _companyManager.UpdateAsync(
            id,
            input.IdentityUserIds, input.Code, input.Name, input.Address1, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Company, CompanyDto>(company);
        }
    }
}