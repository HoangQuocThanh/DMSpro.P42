using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using DMSpro.P42.MDM.Companies;

namespace DMSpro.P42.MDM.Companies
{
    [RemoteService(Name = "MDM")]
    [Area("mDM")]
    [ControllerName("Company")]
    [Route("api/m-d-m/companies")]
    public class CompanyController : AbpController, ICompaniesAppService
    {
        private readonly ICompaniesAppService _companiesAppService;

        public CompanyController(ICompaniesAppService companiesAppService)
        {
            _companiesAppService = companiesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<CompanyDto>> GetListAsync(GetCompaniesInput input)
        {
            return _companiesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<CompanyDto> GetAsync(Guid id)
        {
            return _companiesAppService.GetAsync(id);
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