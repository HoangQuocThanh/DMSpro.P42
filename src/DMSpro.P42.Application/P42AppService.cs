using DMSpro.P42.Localization;
using Volo.Abp.Application.Services;

namespace DMSpro.P42;

/* Inherit your application services from this class.
 */
public abstract class P42AppService : ApplicationService
{
    protected P42AppService()
    {
        LocalizationResource = typeof(P42Resource);
    }
}
