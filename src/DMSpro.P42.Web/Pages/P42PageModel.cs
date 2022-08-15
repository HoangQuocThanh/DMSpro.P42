using DMSpro.P42.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace DMSpro.P42.Web.Pages;

/* Inherit your Page Model classes from this class.
 */
public abstract class P42PageModel : AbpPageModel
{
    protected P42PageModel()
    {
        LocalizationResourceType = typeof(P42Resource);
    }
}
