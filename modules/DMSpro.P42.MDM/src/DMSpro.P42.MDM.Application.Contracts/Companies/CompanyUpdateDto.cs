using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace DMSpro.P42.MDM.Companies
{
    public class CompanyUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        [StringLength(CompanyConsts.CodeMaxLength, MinimumLength = CompanyConsts.CodeMinLength)]
        public string Code { get; set; }
        [Required]
        [StringLength(CompanyConsts.NameMaxLength)]
        public string Name { get; set; }
        public string Address1 { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}