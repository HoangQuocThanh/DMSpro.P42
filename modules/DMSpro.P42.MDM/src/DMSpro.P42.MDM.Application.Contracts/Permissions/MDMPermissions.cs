using Volo.Abp.Reflection;

namespace DMSpro.P42.MDM.Permissions;

public class MDMPermissions
{
    public const string GroupName = "MDM";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(MDMPermissions));
    }

    public class Companies
    {
        public const string Default = GroupName + ".Companies";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
        public const string Import = Default + ".Import";
    }
    //public static class Users
    //{
    //    public const string Default = GroupName + ".Users";
    //    public const string Create = Default + ".Create";
    //    public const string Update = Default + ".Update";
    //    public const string Delete = Default + ".Delete";
    //    public const string ManagePermissions = Default + ".ManagePermissions";
    //    public const string ViewChangeHistory = "AuditLogging.ViewChangeHistory:Volo.Abp.Identity.IdentityUser";
    //    public const string Impersonation = Default + ".Impersonation";
    //    public const string Import = Default + ".Import";
    //}
}