using System.Collections.Generic;

namespace DMSpro.P42.MDM.Companies
{
    public static class CompanyConsts
    {
        private const string DefaultSorting = "{0}Code asc,{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Company." : string.Empty);
        }

        public const int CodeMinLength = 0;
        public const int CodeMaxLength = 20;
        public const int NameMaxLength = 200;

        public const string ListCompany = "list_company";
        public const string CurrentCompany = "current_company";
    }
}