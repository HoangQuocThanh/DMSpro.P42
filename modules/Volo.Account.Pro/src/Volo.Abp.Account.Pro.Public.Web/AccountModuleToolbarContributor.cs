using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account.Public.Web.Modules.Account.Components.Toolbar.Impersonation;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.Users;

namespace Volo.Abp.Account.Public.Web;

public class AccountModuleToolbarContributor : IToolbarContributor
{
    public const int Order = 1000000;

    public virtual Task ConfigureToolbarAsync(IToolbarConfigurationContext context)
    {
        if (context.Toolbar.Name != StandardToolbars.Main)
        {
            return Task.CompletedTask;
        }

        //if (!context.ServiceProvider.GetRequiredService<ICurrentUser>().IsAuthenticated)
        //{
        //    context.Toolbar.Items.Add(new ToolbarItem(typeof(UserLoginLinkViewComponent), order: Order));
        //}

        if (context.ServiceProvider.GetRequiredService<ICurrentUser>().FindImpersonatorUserId() != null)
        {
            context.Toolbar.Items.Add(new ToolbarItem(typeof(ImpersonationViewComponent), order: -1));
        }

        return Task.CompletedTask;
    }
}
