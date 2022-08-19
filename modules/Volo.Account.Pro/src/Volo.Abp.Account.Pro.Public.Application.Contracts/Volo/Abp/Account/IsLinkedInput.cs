using System;

namespace Volo.Abp.Account;

public class IsLinkedInput
{
    public Guid UserId { get; set; }

    public Guid? TenantId { get; set; }
}
