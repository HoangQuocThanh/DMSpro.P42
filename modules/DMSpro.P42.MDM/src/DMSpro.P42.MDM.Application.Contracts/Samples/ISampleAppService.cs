using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace DMSpro.P42.MDM.Samples;

public interface ISampleAppService : IApplicationService
{
    Task<SampleDto> GetAsync();

    Task<SampleDto> GetAuthorizedAsync();
}
