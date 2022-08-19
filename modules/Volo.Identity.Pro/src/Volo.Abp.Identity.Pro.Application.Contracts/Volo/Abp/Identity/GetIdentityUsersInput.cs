using System;
using Volo.Abp.Application.Dtos;

namespace Volo.Abp.Identity;

public class GetIdentityUsersInput : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }

    public Guid? RoleId { get; set; } = null;

    public Guid? OrganizationUnitId { get; set; } = null;

    public string UserName { get; set; }

    public string PhoneNumber { get; set; }

    public string EmailAddress { get; set; }

    public bool? IsLockedOut { get; set; }

    public bool? NotActive { get; set; }
}
