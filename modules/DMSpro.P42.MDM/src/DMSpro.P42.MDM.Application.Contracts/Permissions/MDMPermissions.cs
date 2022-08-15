using Volo.Abp.Reflection;

namespace DMSpro.P42.MDM.Permissions;

public class MDMPermissions
{
    public const string GroupName = "MDM";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(MDMPermissions));
    }
}
