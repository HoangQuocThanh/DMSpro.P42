namespace Volo.Abp.Identity;

public class ExternalLoginProviderDto
{
    public string Name { get; set; }
    
    public bool CanObtainUserInfoWithoutPassword { get; set; }

    public ExternalLoginProviderDto(string name, bool canObtainUserInfoWithoutPassword)
    {
        Name = name;
        CanObtainUserInfoWithoutPassword = canObtainUserInfoWithoutPassword;
    }
}