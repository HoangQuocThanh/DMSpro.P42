using Volo.Abp.Identity.Features;

namespace Volo.Abp.Account;

public class AccountTwoFactorSettingsDto
{
    public IdentityProTwoFactorBehaviour TwoFactorBehaviour { get; set; }

    public bool IsRememberBrowserEnabled { get; set; }

    public bool UsersCanChange { get; set; }
}
