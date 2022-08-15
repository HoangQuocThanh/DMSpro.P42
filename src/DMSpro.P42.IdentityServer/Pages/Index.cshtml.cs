using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.IdentityServer.Clients;

namespace DMSpro.P42.Pages;

public class IndexModel : AbpPageModel
{
    public List<Client> Clients { get; protected set; }

    // TODO: Consider using IClientStore here.
    protected IClientRepository ClientRepository { get; }

    public IndexModel(IClientRepository clientRepository)
    {
        ClientRepository = clientRepository;
    }

    public async Task OnGetAsync()
    {
        Clients = await ClientRepository.GetListAsync(includeDetails: true);
    }
}
