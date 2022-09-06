using DMSpro.P42.eRoute.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace DMSpro.P42.eRoute;

public abstract class eRouteController : AbpControllerBase
{
    protected eRouteController()
    {
        LocalizationResource = typeof(eRouteResource);
    }
}
