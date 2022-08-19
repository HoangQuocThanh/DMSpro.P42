using DMSpro.P42.MDM.Shared;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DMSpro.P42.MDM.Companies;

namespace DMSpro.P42.MDM.Web.Pages.MDM.Companies
{
    public class CreateModalModel : MDMPageModel
    {
        [BindProperty]
        public CompanyCreateDto Company { get; set; }

        [BindProperty]
        public List<Guid> SelectedIdentityUserIds { get; set; }

        private readonly ICompaniesAppService _companiesAppService;

        public CreateModalModel(ICompaniesAppService companiesAppService)
        {
            _companiesAppService = companiesAppService;
        }

        public async Task OnGetAsync()
        {
            Company = new CompanyCreateDto();

            await Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostAsync()
        {

            Company.IdentityUserIds = SelectedIdentityUserIds;

            await _companiesAppService.CreateAsync(Company);
            return NoContent();
        }
    }
}