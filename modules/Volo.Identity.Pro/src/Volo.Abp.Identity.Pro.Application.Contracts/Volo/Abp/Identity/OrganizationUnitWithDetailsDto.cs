using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Volo.Abp.Identity;

public class OrganizationUnitWithDetailsDto : ExtensibleFullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public Guid? ParentId { get; set; }

    public string Code { get; set; }

    public string DisplayName { get; set; }

    public List<IdentityRoleDto> Roles { get; set; }

    public string ConcurrencyStamp { get; set; }
}
