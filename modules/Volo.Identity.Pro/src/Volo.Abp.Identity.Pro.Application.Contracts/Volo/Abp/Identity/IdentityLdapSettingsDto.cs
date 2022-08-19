using System.ComponentModel.DataAnnotations;

namespace Volo.Abp.Identity;

public class IdentityLdapSettingsDto
{
    [Display(Name = "DisplayName:Abp.Identity.EnableLdapLogin")]
    public bool EnableLdapLogin { get; set; }

    [Display(Name = "Abp.Ldap.ServerHost")]
    public string LdapServerHost { get; set; }

    [Display(Name = "Abp.Ldap.ServerPort")]
    public string LdapServerPort { get; set; }

    [Display(Name = "Abp.Ldap.BaseDc")]
    public string LdapBaseDc { get; set; }

    [Display(Name = "Abp.Ldap.Domain")]
    public string LdapDomain { get; set; }

    [Display(Name = "Abp.Ldap.UserName")]
    public string LdapUserName { get; set; }

    [Display(Name = "Abp.Ldap.LdapPassword")]
    public string LdapPassword { get; set; }
}
