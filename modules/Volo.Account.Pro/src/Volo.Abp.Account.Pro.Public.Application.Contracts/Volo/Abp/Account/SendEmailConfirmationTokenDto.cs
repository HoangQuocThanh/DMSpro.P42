using System;
using System.ComponentModel.DataAnnotations;

namespace Volo.Abp.Account;

public class SendEmailConfirmationTokenDto
{
    [Required]
    public string AppName { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public string ReturnUrl { get; set; }

    public string ReturnUrlHash { get; set; }
}
