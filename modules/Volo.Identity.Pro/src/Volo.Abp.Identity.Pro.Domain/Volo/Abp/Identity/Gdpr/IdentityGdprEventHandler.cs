using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Gdpr;
using Volo.Abp.Uow;

namespace Volo.Abp.Identity.Gdpr;

public class IdentityGdprEventHandler : 
    IDistributedEventHandler<GdprUserDataDeletionRequestedEto>, 
    IDistributedEventHandler<GdprUserDataRequestedEto>,
    ITransientDependency
{
    protected IdentityUserManager IdentityUserManager { get; }
    protected IIdentityUserRepository IdentityUserRepository { get; }
    protected IDistributedEventBus EventBus { get; }

    public IdentityGdprEventHandler(
        IdentityUserManager identityUserManager, 
        IIdentityUserRepository identityUserRepository, 
        IDistributedEventBus eventBus)
    {
        IdentityUserManager = identityUserManager;
        IdentityUserRepository = identityUserRepository;
        EventBus = eventBus;
    }

    [UnitOfWork]
    public virtual async Task HandleEventAsync(GdprUserDataDeletionRequestedEto eventData)
    {
        var identityUser = await IdentityUserManager.GetByIdAsync(eventData.UserId);
        identityUser.Name = "";
        identityUser.Surname = "";
        identityUser.SetIsActive(false);

        await IdentityUserManager.SetPhoneNumberAsync(identityUser, "");
        await IdentityUserManager.SetEmailAsync(identityUser, "");
        
        await IdentityUserRepository.UpdateAsync(identityUser);
        await IdentityUserRepository.DeleteAsync(identityUser);
    }

    public virtual async Task HandleEventAsync(GdprUserDataRequestedEto eventData)
    {
        var identityUser = await IdentityUserRepository.GetAsync(eventData.UserId);
        
        var gdprDataInfo = new GdprDataInfo 
        {
            {"Username", identityUser.UserName}, 
            {"Name", identityUser.Name}, 
            {"Surname", identityUser.Surname},
            {"Email", identityUser.Email},
            {"Phone Number", identityUser.PhoneNumber}
        };

        var claims = identityUser.Claims;
        foreach (var identityUserClaim in claims)
        {
            gdprDataInfo.Add(identityUserClaim.ClaimType, identityUserClaim.ClaimValue);
        }

        await EventBus.PublishAsync(
            new GdprUserDataPreparedEto
            {
                RequestId = eventData.RequestId, 
                Data = gdprDataInfo,
                Provider = GetType().FullName
            });
    }
}