using DMSpro.P42.MDM.Shared;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using DMSpro.P42.MDM.Companies;
using System.Collections.Generic;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Http;
using DevExtreme.AspNet.Data.ResponseModel;

namespace DMSpro.P42.MDM.Companies
{
    [RemoteService(Name = "MDM")]
    [Area("mDM")]
    [ControllerName("Company")]
    [Route("api/mdm/companies")]
    public class CompanyController : AbpController, ICompaniesAppService
    {
        private readonly ICompaniesAppService _companiesAppService;

        public CompanyController(ICompaniesAppService companiesAppService)
        {
            _companiesAppService = companiesAppService;
        }

        /*ThanhHQ*/
        //[HttpPost("get-all")]
        [HttpGet]
        public async Task<LoadResult> GetAllAsync(DataSourceLoadOptionsBase loadOptions)
        {
            return await _companiesAppService.GetAllAsync(loadOptions);
        }

        //public async Task<LoadResult> GetAllAsync(DataSourceLoadOptions loadOptions)
        //{
        //    return await _companiesAppService.GetAllAsync(loadOptions);
        //}

        [HttpGet("input")]
        public Task<PagedResultDto<CompanyWithNavigationPropertiesDto>> GetListAsync(GetCompaniesInput input)
        {
            return _companiesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<CompanyWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _companiesAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<CompanyDto> GetAsync(Guid id)
        {
            return _companiesAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("identity-user-lookup")]
        public Task<PagedResultDto<LookupDto<Guid>>> GetIdentityUserLookupAsync(LookupRequestDto input)
        {
            return _companiesAppService.GetIdentityUserLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<CompanyDto> CreateAsync(CompanyCreateDto input)
        {
            return _companiesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<CompanyDto> UpdateAsync(Guid id, CompanyUpdateDto input)
        {
            return _companiesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _companiesAppService.DeleteAsync(id);
        }

    }
}