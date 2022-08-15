using DMSpro.P42.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace DMSpro.P42.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class P42Controller : AbpControllerBase
{
    protected P42Controller()
    {
        LocalizationResource = typeof(P42Resource);
    }
}
