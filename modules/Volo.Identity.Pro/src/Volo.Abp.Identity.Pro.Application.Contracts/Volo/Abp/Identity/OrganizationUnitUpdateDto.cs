using Volo.Abp.Domain.Entities;

namespace Volo.Abp.Identity;

public class OrganizationUnitUpdateDto : OrganizationUnitCreateOrUpdateDtoBase, IHasConcurrencyStamp
{
    public string ConcurrencyStamp { get; set; }
}
