using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DMSpro.P42.MDM.Companies
{
    public class CompanyDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}