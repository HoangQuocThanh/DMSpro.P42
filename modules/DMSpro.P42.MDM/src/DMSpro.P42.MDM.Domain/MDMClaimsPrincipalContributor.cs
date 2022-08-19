using DMSpro.P42.MDM.Companies;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;

namespace DMSpro.P42.MDM;

public class MDMClaimsPrincipalContributor : IAbpClaimsPrincipalContributor, ITransientDependency
{
    public async Task ContributeAsync(AbpClaimsPrincipalContributorContext context)
    {
        var identity = context.ClaimsPrincipal.Identities.FirstOrDefault();
        var userId = identity?.FindUserId();
        if (userId.HasValue)
        {
            var companyService = context.ServiceProvider.GetRequiredService<ICompanyRepository>();
            var listComp = await companyService.GetListAsync();
            Company currentComp = listComp?.FirstOrDefault();

            identity.AddClaim(new Claim(CompanyConsts.ListCompany, JsonSerializer.Serialize(listComp)));
            identity.AddClaim(new Claim(CompanyConsts.CurrentCompany, JsonSerializer.Serialize(currentComp)));
        }
    }
}

public static class CurrentUserExtensions
{
    public static string GetListCompany(this ICurrentUser currentUser)
    {
        return currentUser.FindClaimValue(CompanyConsts.ListCompany);
    }
    public static string GetCurrentCompany(this ICurrentUser currentUser)
    {
        return currentUser.FindClaimValue(CompanyConsts.CurrentCompany);
    }
}
