using PX.Data;
using PX.Objects.SO;
using System;

namespace SWKTechCustomization
{
    public class SOLineExt : PXCacheExtension<SOLine>
    {
        public static bool IsActive() => true;

        #region UsrSWKRTHCost
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "RTH Cost", Enabled = false)]
        public virtual decimal? UsrSWKRTHCost { get; set; }
        public abstract class usrSWKRTHCost : PX.Data.BQL.BqlDecimal.Field<usrSWKRTHCost> { }
        #endregion

        #region UsrSWKSPCCost
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "SPC Cost")]
        public virtual decimal? UsrSWKSPCCost { get; set; }
        public abstract class usrSWKSPCCost : PX.Data.BQL.BqlDecimal.Field<usrSWKSPCCost> { }
        #endregion

        #region UsrSWKSPCCode
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "SPC Code")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXUIRequired(typeof(Where<SOLineExt.usrSWKSPCCost, Greater<decimal0>>))]
        public virtual string UsrSWKSPCCode { get; set; }
        public abstract class usrSWKSPCCode : PX.Data.BQL.BqlString.Field<usrSWKSPCCode> { }
        #endregion

        #region UsrSWKManualCost
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Manual Cost", Enabled = false)]
        public virtual bool? UsrSWKManualCost { get; set; }
        public abstract class usrSWKManualCost : PX.Data.BQL.BqlBool.Field<usrSWKManualCost> { }
        #endregion
    }
}