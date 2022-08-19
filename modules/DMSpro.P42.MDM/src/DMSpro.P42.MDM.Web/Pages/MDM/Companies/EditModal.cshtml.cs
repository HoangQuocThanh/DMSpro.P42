using DMSpro.P42.MDM.Shared;
using Volo.Abp.Identity;
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

        public List<IdentityUserDto> IdentityUsers { get; set; }
        [BindProperty]
        public List<Guid> SelectedIdentityUserIds { get; set; }

        private readonly ICompaniesAppService _companiesAppService;

        public EditModalModel(ICompaniesAppService companiesAppService)
        {
            _companiesAppService = companiesAppService;
        }

        public async Task OnGetAsync()
        {
            var companyWithNavigationPropertiesDto = await _companiesAppService.GetWithNavigationPropertiesAsync(Id);
            Company = ObjectMapper.Map<CompanyDto, CompanyUpdateDto>(companyWithNavigationPropertiesDto.Company);

            IdentityUsers = companyWithNavigationPropertiesDto.IdentityUsers;

        }

        public async Task<NoContentResult> OnPostAsync()
        {

            Company.IdentityUserIds = SelectedIdentityUserIds;

            await _companiesAppService.UpdateAsync(Id, Company);
            return NoContent();
        }
    }
}