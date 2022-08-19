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
        companyPermission.AddChild(MDMPermissions.Companies.Import, L("Permission:Import"));

        //var usersPermission = myGroup.AddPermission(MDMPermissions.Users.Default, L("Permission:UserManagement"));
        //usersPermission.AddChild(MDMPermissions.Users.Create, L("Permission:Create"));
        //usersPermission.AddChild(MDMPermissions.Users.Update, L("Permission:Edit"));
        //usersPermission.AddChild(MDMPermissions.Users.Delete, L("Permission:Delete"));
        //usersPermission.AddChild(MDMPermissions.Users.ManagePermissions, L("Permission:ChangePermissions"));
        //usersPermission.AddChild(MDMPermissions.Users.ViewChangeHistory, L("Permission:ViewChangeHistory"));
        //usersPermission.AddChild(MDMPermissions.Users.Impersonation, L("Permission:Impersonation"));
        //usersPermission.AddChild(MDMPermissions.Users.Import, L("Permission:Import"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MDMResource>(name);
    }
}