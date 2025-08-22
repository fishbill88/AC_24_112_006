using PX.Common;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.SQLTree;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.Objects;
using System.Collections.Generic;
using System;

namespace POInventoryCustomization
{
    public class SOOrderTypeExt : PXCacheExtension<PX.Objects.SO.SOOrderType>
    {
        #region UsrShowVendorID
        [PXDBBool]
        [PXUIField(DisplayName = "Show Vendor")]
        public virtual bool? UsrShowVendorID { get; set; }
        public abstract class usrShowVendorID : PX.Data.BQL.BqlBool.Field<usrShowVendorID> { }
        #endregion

        #region UsrShowVendorLocationID
        [PXDBBool]
        [PXUIField(DisplayName = "Show Vendor Location")]
        public virtual bool? UsrShowVendorLocationID { get; set; }
        public abstract class usrShowVendorLocationID : PX.Data.BQL.BqlBool.Field<usrShowVendorLocationID> { }
        #endregion

        #region UsrShowVendorAddress
        [PXDBBool]
        [PXUIField(DisplayName = "Show Vendor Address")]
        public virtual bool? UsrShowVendorAddress { get; set; }
        public abstract class usrShowVendorAddress : PX.Data.BQL.BqlBool.Field<usrShowVendorAddress> { }
        #endregion
    }
}