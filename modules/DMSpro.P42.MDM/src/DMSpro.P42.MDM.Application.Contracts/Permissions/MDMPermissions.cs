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
    }
    public class Users
    {
        public const string Default = GroupName + ".Users";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
}