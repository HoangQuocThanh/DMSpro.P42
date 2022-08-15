using DMSpro.P42.MDM.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DMSpro.P42.MDM.Permissions;

public class MDMPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(MDMPermissions.GroupName, L("Permission:MDM"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MDMResource>(name);
    }
}
