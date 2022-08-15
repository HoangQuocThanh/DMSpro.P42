using DMSpro.P42.MDM.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace DMSpro.P42.MDM.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class MDMPageModel : AbpPageModel
{
    protected MDMPageModel()
    {
        LocalizationResourceType = typeof(MDMResource);
        ObjectMapperContext = typeof(MDMWebModule);
    }
}
