using System;
using Volo.Abp.Domain.Entities;

namespace DMSpro.P42.MDM.Companies
{
    public class CompanyIdentityUser : Entity
    {

        public Guid CompanyId { get; protected set; }

        public Guid IdentityUserId { get; protected set; }

        private CompanyIdentityUser()
        {

        }

        public CompanyIdentityUser(Guid companyId, Guid identityUserId)
        {
            CompanyId = companyId;
            IdentityUserId = identityUserId;
        }

        public override object[] GetKeys()
        {
            return new object[]
                {
                    CompanyId,
                    IdentityUserId
                };
        }
    }
}