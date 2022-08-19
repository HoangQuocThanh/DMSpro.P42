using Volo.Abp.Content;

namespace Volo.Abp.Account;

public class ProfilePictureInput
{
    public ProfilePictureType Type { get; set; }

    public IRemoteStreamContent ImageContent { get; set; }
}
