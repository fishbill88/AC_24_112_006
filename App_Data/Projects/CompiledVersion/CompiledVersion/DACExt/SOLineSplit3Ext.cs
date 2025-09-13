using PX.Data;
using PX.Data.BQL;
using PX.Objects.SO;

namespace CompiledVersion.DAC
{
    // Extends PX.Objects.PO.POOrderEntry.SOLineSplit3
    public sealed class SOLineSplit3Ext : PXCacheExtension<PX.Objects.PO.POOrderEntry.SOLineSplit3>
    {
        public static bool IsActive() => true;

        // Unbound field populated from the related SOLine.UsrSWKSPCCode
        //[PXString(30, IsUnicode = true)]
        //[PXUIField(DisplayName = "SPC Code")]
        //[PXDBScalar(typeof(Search<SOLineExt.usrSWKSPCCode,
        //    Where<SOLine.orderType, Equal<Current<PX.Objects.PO.POOrderEntry.SOLineSplit3.orderType>>,
        //      And<SOLine.orderNbr, Equal<Current<PX.Objects.PO.POOrderEntry.SOLineSplit3.orderNbr>>,
        //      And<SOLine.lineNbr, Equal<Current<PX.Objects.PO.POOrderEntry.SOLineSplit3.lineNbr>>>>>>))]
        //public string UsrSWKSPCCode { get; set; }
        //public abstract class usrSWKSPCCode : BqlString.Field<usrSWKSPCCode> { }
    }
}