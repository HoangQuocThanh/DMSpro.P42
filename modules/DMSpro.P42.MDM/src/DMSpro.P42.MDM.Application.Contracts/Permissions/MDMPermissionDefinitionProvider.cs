using DMSpro.P42.MDM.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DMSpro.P42.MDM.Permissions;

public class MDMPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(MDMPermissions.GroupName, L("Permission:MDM"));

        var companyPermission = myGroup.AddPermission(MDMPermissions.Companies.Default, L("Permission:Companies"));
        companyPermission.AddChild(MDMPermissions.Companies.Create, L("Permission:Create"));
        companyPermission.AddChild(MDMPermissions.Companies.Edit, L("Permission:Edit"));
        companyPermission.AddChild(MDMPermissions.Companies.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MDMResource>(name);
    }
}