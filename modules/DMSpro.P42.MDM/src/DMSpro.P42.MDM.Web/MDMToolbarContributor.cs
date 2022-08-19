using DMSpro.P42.MDM.Web.Pages.MDM.Companies.ImportToolbar;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;

namespace DMSpro.P42.MDM.Web
{
    public class MDMToolbarContributor : IToolbarContributor
    {
        public Task ConfigureToolbarAsync(IToolbarConfigurationContext context)
        {
            if (context.Toolbar.Name == StandardToolbars.Main)
            {
                context.Toolbar.Items
                    .Insert(0, new ToolbarItem(typeof(ImportDropdownViewComponent)));
            }

            return Task.CompletedTask;
        }
    }
}
