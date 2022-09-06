using DMSpro.P42.eRoute.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DMSpro.P42.eRoute.Permissions;

public class eRoutePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(eRoutePermissions.GroupName, L("Permission:eRoute"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<eRouteResource>(name);
    }
}
