using DMSpro.P42.SO.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DMSpro.P42.SO.Permissions;

public class SOPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(SOPermissions.GroupName, L("Permission:SO"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<SOResource>(name);
    }
}
