using PX.Data;
using PX.Objects.CR;
using PX.Objects.CR.MassProcess;

namespace QLTenantCopyItems
{
    public sealed class QLContactExtension : PXCacheExtension<Contact>
    {
        public static bool IsActive() => true;

        [PXContactInfoField]
        [PXDBString(100, IsUnicode = true)]
        [PXMassMergableField]
        [PXPersonalDataField]
        [PXUIField(DisplayName = "Last Name")]
        public string LastName
        {
            get;
            set;
        }
    }
}