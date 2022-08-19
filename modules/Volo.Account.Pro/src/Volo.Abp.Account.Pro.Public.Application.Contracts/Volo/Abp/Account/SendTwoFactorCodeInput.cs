using System;
using System.ComponentModel.DataAnnotations;

namespace Volo.Abp.Account;

public class SendTwoFactorCodeInput
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Provider { get; set; }

    [Required]
    public string Token { get; set; }
}
