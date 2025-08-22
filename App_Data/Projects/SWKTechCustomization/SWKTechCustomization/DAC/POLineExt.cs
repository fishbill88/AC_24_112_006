using PX.Data;
using PX.Objects.PO;
using System;

namespace SWKTechCustomization
{
    public class POLineExt : PXCacheExtension<POLine>
    {
        public static bool IsActive() => true;

        #region UsrSWKRTHCost
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "RTH Cost", Enabled = false)]
        public virtual decimal? UsrSWKRTHCost { get; set; }
        public abstract class usrSWKRTHCost : PX.Data.BQL.BqlDecimal.Field<usrSWKRTHCost> { }
        #endregion

        #region UsrSWKSPCCode
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "SPC Code",Enabled = false)]
        public virtual string UsrSWKSPCCode { get; set; }
        public abstract class usrSWKSPCCode : PX.Data.BQL.BqlString.Field<usrSWKSPCCode> { }
        #endregion
    }
}