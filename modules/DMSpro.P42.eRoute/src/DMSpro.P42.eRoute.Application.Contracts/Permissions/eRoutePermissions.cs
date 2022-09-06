using Volo.Abp.Reflection;

namespace DMSpro.P42.eRoute.Permissions;

public class eRoutePermissions
{
    public const string GroupName = "eRoute";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(eRoutePermissions));
    }
}
