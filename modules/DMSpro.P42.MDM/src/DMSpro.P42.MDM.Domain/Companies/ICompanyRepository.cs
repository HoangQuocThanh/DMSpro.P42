using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DMSpro.P42.MDM.Companies
{
    public interface ICompanyRepository : IRepository<Company, Guid>
    {
        Task<List<Company>> GetListAsync(
            string filterText = null,
            string code = null,
            string name = null,
            string address1 = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string code = null,
            string name = null,
            string address1 = null,
            CancellationToken cancellationToken = default);
    }
}