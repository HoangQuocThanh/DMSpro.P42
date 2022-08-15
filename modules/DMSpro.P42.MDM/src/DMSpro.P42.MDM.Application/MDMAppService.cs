using DMSpro.P42.MDM.Localization;
using Volo.Abp.Application.Services;

namespace DMSpro.P42.MDM;

public abstract class MDMAppService : ApplicationService
{
    protected MDMAppService()
    {
        LocalizationResource = typeof(MDMResource);
        ObjectMapperContext = typeof(MDMApplicationModule);
    }
}
