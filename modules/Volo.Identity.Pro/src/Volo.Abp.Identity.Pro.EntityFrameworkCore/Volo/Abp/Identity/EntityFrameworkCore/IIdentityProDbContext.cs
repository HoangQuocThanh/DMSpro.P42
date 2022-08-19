using Volo.Abp.Data;

namespace Volo.Abp.Identity.EntityFrameworkCore;

[ConnectionStringName(AbpIdentityDbProperties.ConnectionStringName)]
public interface IIdentityProDbContext : IIdentityDbContext
{

}
