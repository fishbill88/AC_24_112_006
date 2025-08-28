using PX.Data;

namespace CompiledVersion.DAC
{
    public sealed class SOOrderTypeExt : PXCacheExtension<PX.Objects.SO.SOOrderType>
    {
        public static bool IsActive() => true;

        #region UsrShowVendorID
        [PXDBBool]
        [PXUIField(DisplayName = "Show Vendor")]
        public bool? UsrShowVendorID { get; set; }
        public abstract class usrShowVendorID : PX.Data.BQL.BqlBool.Field<usrShowVendorID> { }
        #endregion

        #region UsrShowVendorLocationID
        [PXDBBool]
        [PXUIField(DisplayName = "Show Vendor Location")]
        public bool? UsrShowVendorLocationID { get; set; }
        public abstract class usrShowVendorLocationID : PX.Data.BQL.BqlBool.Field<usrShowVendorLocationID> { }
        #endregion

        #region UsrShowVendorAddress
        [PXDBBool]
        [PXUIField(DisplayName = "Show Vendor Address")]
        public bool? UsrShowVendorAddress { get; set; }
        public abstract class usrShowVendorAddress : PX.Data.BQL.BqlBool.Field<usrShowVendorAddress> { }
        #endregion
    }
}