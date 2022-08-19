using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Settings;

namespace Volo.Abp.Account.Public.Web.Pages.Account;

public class ConfirmUserModel : AccountPageModel
{
    public static string ConfirmUserScheme = "Abp.ConfirmUser";

    public UserInfoModel UserInfo { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [Required]
    public string PhoneConfirmationToken { get; set; }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        UserInfo = await RetrieveConfirmUser();

        if (UserInfo == null)
        {
            return await RedirectToLoginPageAsync();
        }

        if (UserInfo.TenantId != CurrentTenant.Id)
        {
            return await RedirectToLoginPageAsync();
        }

        if (UserInfo.PhoneNumberConfirmed &&
            UserInfo.EmailConfirmed)
        {
            return await RedirectToLoginPageAsync();
        }

        if (UserInfo.RequireConfirmedEmail &&
            UserInfo.EmailConfirmed &&
            !UserInfo.RequireConfirmedPhoneNumber)
        {
            return await RedirectToLoginPageAsync();
        }

        if (UserInfo.RequireConfirmedPhoneNumber &&
            UserInfo.PhoneNumberConfirmed &&
            !UserInfo.RequireConfirmedEmail)
        {
            return await RedirectToLoginPageAsync();
        }

        return Page();
    }

    protected virtual async Task<IActionResult> RedirectToLoginPageAsync()
    {
        if (UserInfo != null)
        {
            // Try to cleanup confirm user id cookies
            await HttpContext.SignOutAsync(ConfirmUserModel.ConfirmUserScheme);
        }

        return RedirectToPage("./Login", new {
            ReturnUrl = ReturnUrl,
            ReturnUrlHash = ReturnUrlHash
        });
    }

    protected virtual async Task<UserInfoModel> RetrieveConfirmUser()
    {
        var result = await HttpContext.AuthenticateAsync(ConfirmUserModel.ConfirmUserScheme);
        if (result?.Principal != null)
        {
            var userId = result.Principal.FindUserId();
            if (userId == null)
            {
                return null;
            }

            var tenantId = result.Principal.FindTenantId();

            using (CurrentTenant.Change(tenantId))
            {
                var user = await UserManager.FindByIdAsync(userId.Value.ToString());
                if (user == null)
                {
                    return null;
                }

                return new UserInfoModel
                {
                    Id = user.Id,
                    TenantId = user.TenantId,

                    RequireConfirmedEmail = await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.RequireConfirmedEmail),
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,

                    RequireConfirmedPhoneNumber = await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.RequireConfirmedPhoneNumber),
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                };
            }
        }

        return null;
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

    public class UserInfoModel
    {
        public Guid Id { get; set; }

        public Guid? TenantId { get; set; }

        public bool RequireConfirmedEmail { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool RequireConfirmedPhoneNumber { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }
    }
}
