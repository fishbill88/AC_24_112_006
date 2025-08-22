using PX.Data;
using PX.Objects.PO;

namespace SWKTechCustomization
{
    public class POLineExt : PXCacheExtension<POLine>
    {
        public static bool IsActive() => true;

        #region UsrSWKSPCCode
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "SPC Code",Enabled = false)]
        public virtual string UsrSWKSPCCode { get; set; }
        public abstract class usrSWKSPCCode : PX.Data.BQL.BqlString.Field<usrSWKSPCCode> { }
        #endregion
    }
}