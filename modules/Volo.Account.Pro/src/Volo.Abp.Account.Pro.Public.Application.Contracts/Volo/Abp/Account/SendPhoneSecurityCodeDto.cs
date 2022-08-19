using System;
using System.ComponentModel.DataAnnotations;

namespace Volo.Abp.Account;

public class SendPhoneSecurityCodeDto
{
    [Required]
    public Guid UserId { get; set; }
}
