using Volo.Abp.Identity;
using System.Collections.Generic;

namespace DMSpro.P42.MDM.Companies
{
    public class CompanyWithNavigationProperties
    {
        public Company Company { get; set; }
        public List<IdentityUser> IdentityUsers { get; set; }
        
    }
}