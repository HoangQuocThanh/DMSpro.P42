using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Settings;

namespace Volo.Abp.Account.Emailing;

public class AccountIdentityUserCreatedEventHandler :
    IDistributedEventHandler<IdentityUserCreatedEto>,
    ITransientDependency
{
    protected IdentityUserManager UserManager { get; }
    protected IAccountEmailer AccountEmailer { get; }
    protected ISettingProvider SettingProvider { get; }

    public AccountIdentityUserCreatedEventHandler(
        IdentityUserManager userManager,
        IAccountEmailer accountEmailer,
        ISettingProvider settingProvider)
    {
        UserManager = userManager;
        AccountEmailer = accountEmailer;
        SettingProvider = settingProvider;
    }

    public async Task HandleEventAsync(IdentityUserCreatedEto eventData)
    {
        if (eventData.Properties["SendConfirmationEmail"] == true.ToString().ToUpper() &&
            await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.RequireConfirmedEmail))
        {
            var user = await UserManager.GetByIdAsync(eventData.Id);
            var confirmationToken = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            await AccountEmailer.SendEmailConfirmationLinkAsync(user, confirmationToken,
                eventData.Properties.GetOrDefault("AppName") ?? "MVC");
        }
    }
}
