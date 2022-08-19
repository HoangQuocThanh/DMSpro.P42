using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Volo.Abp.Account.Public.Web.Pages.Account;

public class SecurityLogsModel : AccountPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [Display(Name = "MySecurityLogs:StartTime")]
    public DateTime? StartTime { get; set; }

    [Display(Name = "MySecurityLogs:EndTime")]
    public DateTime? EndTime { get; set; }

    [Display(Name = "MySecurityLogs:Action")]
    public string Action { get; set; }

    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

}
