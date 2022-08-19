using System;
using Volo.Abp.Application.Dtos;

namespace Volo.Abp.Identity;

public class OrganizationUnitLookupDto : EntityDto<Guid>
{
    public string DisplayName { get; set; }
}
