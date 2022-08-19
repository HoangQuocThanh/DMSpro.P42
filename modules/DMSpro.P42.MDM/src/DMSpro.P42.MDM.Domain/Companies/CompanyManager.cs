using Volo.Abp.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DMSpro.P42.MDM.Companies
{
    public class CompanyManager : DomainService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IRepository<IdentityUser, Guid> _identityUserRepository;

        public CompanyManager(ICompanyRepository companyRepository,
        IRepository<IdentityUser, Guid> identityUserRepository)
        {
            _companyRepository = companyRepository;
            _identityUserRepository = identityUserRepository;
        }

        public async Task<Company> CreateAsync(
        List<Guid> identityUserIds,
        string code, string name, string address1)
        {
            var company = new Company(
             GuidGenerator.Create(),
             code, name, address1
             );

            await SetIdentityUsersAsync(company, identityUserIds);

            return await _companyRepository.InsertAsync(company);
        }

        public async Task<Company> UpdateAsync(
            Guid id,
            List<Guid> identityUserIds,
        string code, string name, string address1, [CanBeNull] string concurrencyStamp = null
        )
        {
            var queryable = await _companyRepository.WithDetailsAsync(x => x.IdentityUsers);
            var query = queryable.Where(x => x.Id == id);

            var company = await AsyncExecuter.FirstOrDefaultAsync(query);

            company.Code = code;
            company.Name = name;
            company.Address1 = address1;

            await SetIdentityUsersAsync(company, identityUserIds);

            company.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _companyRepository.UpdateAsync(company);
        }

        private async Task SetIdentityUsersAsync(Company company, List<Guid> identityUserIds)
        {
            if (identityUserIds == null || !identityUserIds.Any())
            {
                company.RemoveAllIdentityUsers();
                return;
            }

            var query = (await _identityUserRepository.GetQueryableAsync())
                .Where(x => identityUserIds.Contains(x.Id))
                .Select(x => x.Id);

            var identityUserIdsInDb = await AsyncExecuter.ToListAsync(query);
            if (!identityUserIdsInDb.Any())
            {
                return;
            }

            company.RemoveAllIdentityUsersExceptGivenIds(identityUserIdsInDb);

            foreach (var identityUserId in identityUserIdsInDb)
            {
                company.AddIdentityUser(identityUserId);
            }
        }

    }
}