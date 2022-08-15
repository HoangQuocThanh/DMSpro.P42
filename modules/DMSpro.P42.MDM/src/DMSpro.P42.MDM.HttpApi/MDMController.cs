using DMSpro.P42.MDM.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace DMSpro.P42.MDM;

public abstract class MDMController : AbpControllerBase
{
    protected MDMController()
    {
        LocalizationResource = typeof(MDMResource);
    }
}
