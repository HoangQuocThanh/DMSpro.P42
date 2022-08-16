using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using DMSpro.P42.MDM.Companies;
using DMSpro.P42.MDM.Shared;

namespace DMSpro.P42.MDM.Web.Pages.MDM.Companies
{
    public class IndexModel : AbpPageModel
    {
        public string CodeFilter { get; set; }
        public string NameFilter { get; set; }
        public string Address1Filter { get; set; }

        private readonly ICompaniesAppService _companiesAppService;

        public IndexModel(ICompaniesAppService companiesAppService)
        {
            _companiesAppService = companiesAppService;
        }

        public async Task OnGetAsync()
        {

            await Task.CompletedTask;
        }
    }
}