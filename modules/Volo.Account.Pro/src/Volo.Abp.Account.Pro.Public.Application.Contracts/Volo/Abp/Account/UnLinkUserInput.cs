using System;

namespace Volo.Abp.Account;

public class UnLinkUserInput
{
    public Guid UserId { get; set; }

    public Guid? TenantId { get; set; }
}
