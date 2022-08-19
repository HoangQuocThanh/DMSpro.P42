using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DMSpro.P42.MDM.Companies
{
    public class Company : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Code { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        [CanBeNull]
        public virtual string Address1 { get; set; }

        public ICollection<CompanyIdentityUser> IdentityUsers { get; private set; }

        public Company()
        {

        }

        public Company(Guid id, string code, string name, string address1)
        {

            Id = id;
            Check.NotNull(code, nameof(code));
            Check.Length(code, nameof(code), CompanyConsts.CodeMaxLength, CompanyConsts.CodeMinLength);
            Check.NotNull(name, nameof(name));
            Check.Length(name, nameof(name), CompanyConsts.NameMaxLength, 0);
            Code = code;
            Name = name;
            Address1 = address1;
            IdentityUsers = new Collection<CompanyIdentityUser>();
        }
        public void AddIdentityUser(Guid identityUserId)
        {
            Check.NotNull(identityUserId, nameof(identityUserId));

            if (IsInIdentityUsers(identityUserId))
            {
                return;
            }

            IdentityUsers.Add(new CompanyIdentityUser(Id, identityUserId));
        }

        public void RemoveIdentityUser(Guid identityUserId)
        {
            Check.NotNull(identityUserId, nameof(identityUserId));

            if (!IsInIdentityUsers(identityUserId))
            {
                return;
            }

            IdentityUsers.RemoveAll(x => x.IdentityUserId == identityUserId);
        }

        public void RemoveAllIdentityUsersExceptGivenIds(List<Guid> identityUserIds)
        {
            Check.NotNullOrEmpty(identityUserIds, nameof(identityUserIds));

            IdentityUsers.RemoveAll(x => !identityUserIds.Contains(x.IdentityUserId));
        }

        public void RemoveAllIdentityUsers()
        {
            IdentityUsers.RemoveAll(x => x.CompanyId == Id);
        }

        private bool IsInIdentityUsers(Guid identityUserId)
        {
            return IdentityUsers.Any(x => x.IdentityUserId == identityUserId);
        }
    }
}