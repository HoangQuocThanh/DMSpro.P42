using System;

namespace Volo.Abp.Account;

[Serializable]
public class ScoreBelowThresholdException : UserFriendlyException
{
    public ScoreBelowThresholdException(string message)
        : base(message)
    {
    }
}
