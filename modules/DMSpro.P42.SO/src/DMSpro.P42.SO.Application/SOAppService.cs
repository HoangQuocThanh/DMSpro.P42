using DMSpro.P42.SO.Localization;
using Volo.Abp.Application.Services;

namespace DMSpro.P42.SO;

public abstract class SOAppService : ApplicationService
{
    protected SOAppService()
    {
        LocalizationResource = typeof(SOResource);
        ObjectMapperContext = typeof(SOApplicationModule);
    }
}
