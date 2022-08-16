using Volo.Abp.Application.Dtos;
using System;

namespace DMSpro.P42.MDM.Companies
{
    public class GetCompaniesInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }

        public GetCompaniesInput()
        {

        }
    }
}