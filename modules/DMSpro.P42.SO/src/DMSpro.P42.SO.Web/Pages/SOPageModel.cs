using DMSpro.P42.SO.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace DMSpro.P42.SO.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class SOPageModel : AbpPageModel
{
    protected SOPageModel()
    {
        LocalizationResourceType = typeof(SOResource);
        ObjectMapperContext = typeof(SOWebModule);
    }
}
