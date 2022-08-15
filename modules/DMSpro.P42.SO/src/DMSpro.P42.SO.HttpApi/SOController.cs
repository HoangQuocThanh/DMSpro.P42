using DMSpro.P42.SO.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace DMSpro.P42.SO;

public abstract class SOController : AbpControllerBase
{
    protected SOController()
    {
        LocalizationResource = typeof(SOResource);
    }
}
