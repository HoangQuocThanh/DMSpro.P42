using Volo.Abp.Reflection;

namespace DMSpro.P42.SO.Permissions;

public class SOPermissions
{
    public const string GroupName = "SO";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(SOPermissions));
    }
}
