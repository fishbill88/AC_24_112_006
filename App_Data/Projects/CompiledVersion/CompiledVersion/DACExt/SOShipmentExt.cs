using PX.Data;

namespace CompiledVersion.DAC
{
    public sealed class SOShipmentExt : PXCacheExtension<PX.Objects.SO.SOShipment>
    {
        public static bool IsActive() => true;

        #region UsrShippingNotes
        [PXDBString(500, IsUnicode = true)]
        [PXUIField(DisplayName = "Shipping Notes")]
        public string UsrShippingNotes { get; set; }
        public abstract class usrShippingNotes : PX.Data.BQL.BqlString.Field<usrShippingNotes> { }
        #endregion

        #region UsrReceiptNbr
        [PXString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Receipt Nbr")]
        public string UsrReceiptNbr { get; set; }
        public abstract class usrReceiptNbr : PX.Data.BQL.BqlString.Field<usrReceiptNbr> { }
        #endregion
    }
}