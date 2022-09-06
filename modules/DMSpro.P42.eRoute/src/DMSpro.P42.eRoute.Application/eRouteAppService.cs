using DMSpro.P42.eRoute.Localization;
using Volo.Abp.Application.Services;

namespace DMSpro.P42.eRoute;

public abstract class eRouteAppService : ApplicationService
{
    protected eRouteAppService()
    {
        LocalizationResource = typeof(eRouteResource);
        ObjectMapperContext = typeof(eRouteApplicationModule);
    }
}
