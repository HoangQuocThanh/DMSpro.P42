using DMSpro.P42.MDM.Shared;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using DMSpro.P42.MDM.Companies;

namespace DMSpro.P42.MDM.Web.Pages.MDM.Companies
{
    public class EditModalModel : MDMPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public CompanyUpdateDto Company { get; set; }

        private readonly ICompaniesAppService _companiesAppService;

        public EditModalModel(ICompaniesAppService companiesAppService)
        {
            _companiesAppService = companiesAppService;
        }

        public async Task OnGetAsync()
        {
            var company = await _companiesAppService.GetAsync(Id);
            Company = ObjectMapper.Map<CompanyDto, CompanyUpdateDto>(company);

        }

        public async Task<NoContentResult> OnPostAsync()
        {

            await _companiesAppService.UpdateAsync(Id, Company);
            return NoContent();
        }
    }
}