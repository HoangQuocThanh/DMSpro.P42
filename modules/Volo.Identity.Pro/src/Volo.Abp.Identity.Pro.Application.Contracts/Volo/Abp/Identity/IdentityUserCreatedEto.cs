using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace Volo.Abp.Identity;

[Serializable]
public class IdentityUserCreatedEto : EtoBase
{
    public Guid Id { get; set; }
}
