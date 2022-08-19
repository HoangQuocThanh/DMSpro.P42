using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Volo.Abp.Account.Public.Web.Pages.Account;

public class EmailConfirmationModel : AccountPageModel
{
    private readonly IAccountAppService _accountAppService;

    public EmailConfirmationModel(IAccountAppService accountAppService)
    {
        _accountAppService = accountAppService;
    }

    [Required]
    [BindProperty(SupportsGet = true)]
    public Guid UserId { get; set; }

    [Required]
    [BindProperty(SupportsGet = true)]
    public string ConfirmationToken { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }
    
    public bool EmailConfirmed { get; set; }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        ReturnUrl = GetRedirectUrl(ReturnUrl, ReturnUrlHash);

        try
        {
            await _accountAppService.ConfirmEmailAsync(new ConfirmEmailInput
            {
                UserId = UserId,
                Token = ConfirmationToken
            });
        }
        catch (Exception e)
        {
            Alerts.Danger(GetLocalizeExceptionMessage(e));
            return Page();
        }

        EmailConfirmed = true;
        return Page();
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }
}
