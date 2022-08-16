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

        public CompanyManager(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<Company> CreateAsync(
        string code, string name, string address1)
        {
            var company = new Company(
             GuidGenerator.Create(),
             code, name, address1
             );

            return await _companyRepository.InsertAsync(company);
        }

        public async Task<Company> UpdateAsync(
            Guid id,
            string code, string name, string address1, [CanBeNull] string concurrencyStamp = null
        )
        {
            var queryable = await _companyRepository.GetQueryableAsync();
            var query = queryable.Where(x => x.Id == id);

            var company = await AsyncExecuter.FirstOrDefaultAsync(query);

            company.Code = code;
            company.Name = name;
            company.Address1 = address1;

            company.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _companyRepository.UpdateAsync(company);
        }

    }
}