using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Emailing;
using Volo.Abp.Sms;
using Volo.Abp.Uow;
using SignInResult = Microsoft.AspNetCore.Mvc.SignInResult;

namespace Volo.Abp.Account.Public.Web.Pages.Account;

public class SendSecurityCodeModel : AccountPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public bool RememberMe { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? LinkUserId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? LinkTenantId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string LinkToken { get; set; }

    public List<SelectListItem> Providers { get; set; }

    [BindProperty]
    public string SelectedProvider { get; set; }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnGetAsync()
    {
        var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            return RedirectToPage("./Login");
        }

        CheckCurrentTenant(user.TenantId);
        //TODO: CheckCurrentTenant(await SignInManager.GetVerifiedTenantIdAsync()); ???

        Providers = (await AccountAppService.GetTwoFactorProvidersAsync(new GetTwoFactorProvidersInput
        {
            UserId = user.Id,
            Token = await UserManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider,
                nameof(Microsoft.AspNetCore.Identity.SignInResult.RequiresTwoFactor))
        })).Select(userProvider => new SelectListItem
        {
            Text = userProvider,
            Value = userProvider
        }).ToList();

        return Page();
    }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnPostAsync()
    {
        var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        CheckCurrentTenant(user.TenantId);
        //TODO: CheckCurrentTenant(await SignInManager.GetVerifiedTenantIdAsync()); ???


        await AccountAppService.SendTwoFactorCodeAsync(new SendTwoFactorCodeInput()
        {
            UserId = user.Id,
            Provider = SelectedProvider,
            Token = await UserManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider,
                nameof(Microsoft.AspNetCore.Identity.SignInResult.RequiresTwoFactor))
        });

        return RedirectToPage("./VerifySecurityCode", new {
            provider = SelectedProvider,
            returnUrl = ReturnUrl,
            returnUrlHash = ReturnUrlHash,
            rememberMe = RememberMe,
            linkUserId = LinkUserId,
            linkTenantId = LinkTenantId,
            linkToken = LinkToken
        });
    }
}
