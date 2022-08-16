using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using DMSpro.P42.MDM.EntityFrameworkCore;

namespace DMSpro.P42.MDM.Companies
{
    public class EfCoreCompanyRepository : EfCoreRepository<MDMDbContext, Company, Guid>, ICompanyRepository
    {
        public EfCoreCompanyRepository(IDbContextProvider<MDMDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<Company>> GetListAsync(
            string filterText = null,
            string code = null,
            string name = null,
            string address1 = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, code, name, address1);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CompanyConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string code = null,
            string name = null,
            string address1 = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, code, name, address1);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Company> ApplyFilter(
            IQueryable<Company> query,
            string filterText,
            string code = null,
            string name = null,
            string address1 = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Code.Contains(filterText) || e.Name.Contains(filterText) || e.Address1.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                    .WhereIf(!string.IsNullOrWhiteSpace(address1), e => e.Address1.Contains(address1));
        }
    }
}