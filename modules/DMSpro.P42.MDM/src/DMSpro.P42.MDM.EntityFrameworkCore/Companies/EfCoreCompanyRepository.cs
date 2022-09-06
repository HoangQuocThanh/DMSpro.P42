using Volo.Abp.Identity;
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
using DevExtreme.AspNet.Data;

namespace DMSpro.P42.MDM.Companies
{
    public class EfCoreCompanyRepository : EfCoreRepository<MDMDbContext, Company, Guid>, ICompanyRepository
    {
        public EfCoreCompanyRepository(IDbContextProvider<MDMDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<CompanyWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id).Include(x => x.IdentityUsers)
                .Select(company => new CompanyWithNavigationProperties
                {
                    Company = company,
                    IdentityUsers = (from companyIdentityUsers in company.IdentityUsers
                                     join _identityUser in dbContext.Set<IdentityUser>() on companyIdentityUsers.IdentityUserId equals _identityUser.Id
                                     select _identityUser).ToList()
                }).FirstOrDefault();
        }

        public async Task<List<CompanyWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string code = null,
            string name = null,
            string address1 = null,
            Guid? identityUserId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, code, name, address1, identityUserId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CompanyConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<CompanyWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from company in (await GetDbSetAsync())

                   select new CompanyWithNavigationProperties
                   {
                       Company = company,
                       IdentityUsers = new List<IdentityUser>()
                   };
        }

        protected virtual IQueryable<CompanyWithNavigationProperties> ApplyFilter(
            IQueryable<CompanyWithNavigationProperties> query,
            string filterText,
            string code = null,
            string name = null,
            string address1 = null,
            Guid? identityUserId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Company.Code.Contains(filterText) || e.Company.Name.Contains(filterText) || e.Company.Address1.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Company.Code.Contains(code))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Company.Name.Contains(name))
                    .WhereIf(!string.IsNullOrWhiteSpace(address1), e => e.Company.Address1.Contains(address1))
                    .WhereIf(identityUserId != null && identityUserId != Guid.Empty, e => e.Company.IdentityUsers.Any(x => x.IdentityUserId == identityUserId));
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
            Guid? identityUserId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, code, name, address1, identityUserId);
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