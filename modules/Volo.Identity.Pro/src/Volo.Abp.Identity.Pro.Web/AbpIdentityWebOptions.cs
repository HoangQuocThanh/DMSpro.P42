namespace Volo.Abp.Identity.Web;

public class AbpIdentityWebOptions
{
    public bool EnableUserImpersonation { get; set; }

    public AbpIdentityWebOptions()
    {
        EnableUserImpersonation = false;
    }
}
