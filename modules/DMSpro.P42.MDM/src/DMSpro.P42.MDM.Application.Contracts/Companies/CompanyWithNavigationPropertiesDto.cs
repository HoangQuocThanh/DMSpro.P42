using Volo.Abp.Identity;
using System.Collections.Generic;

namespace DMSpro.P42.MDM.Companies
{
    public class CompanyWithNavigationPropertiesDto
    {
        public CompanyDto Company { get; set; }

        public List<IdentityUserDto> IdentityUsers { get; set; }

    }
}