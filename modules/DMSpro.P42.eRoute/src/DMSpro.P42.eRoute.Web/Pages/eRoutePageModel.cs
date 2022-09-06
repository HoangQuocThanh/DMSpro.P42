using DMSpro.P42.eRoute.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace DMSpro.P42.eRoute.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class eRoutePageModel : AbpPageModel
{
    protected eRoutePageModel()
    {
        LocalizationResourceType = typeof(eRouteResource);
        ObjectMapperContext = typeof(eRouteWebModule);
    }
}
