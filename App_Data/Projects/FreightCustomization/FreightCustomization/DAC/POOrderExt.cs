using PX.Data;
using PX.Objects.PO;
using System;

namespace FreightCustomization
{
    public sealed class POOrderExt : PXCacheExtension<POOrder>
    {
        public static bool IsActive() => true;
        #region UsrFreightCost
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Freight Cost")]
        public decimal? UsrFreightCost { get; set; }
        public abstract class usrFreightCost : PX.Data.BQL.BqlDecimal.Field<usrFreightCost> { }
        #endregion

        #region UsrShipTermsIDTemp
        [PXString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXUIField(DisplayName = "Ship Terms ID Temp", Visible = false, Enabled = false)]
        public string UsrShipTermsIDTemp { get; set; }
        public abstract class usrShipTermsIDTemp : PX.Data.BQL.BqlString.Field<usrShipTermsIDTemp> { }
        #endregion

        #region UsrShowFreightCost
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public bool? UsrShowFreightCost { get; set; }
        public abstract class usrShowFreightCost : PX.Data.BQL.BqlBool.Field<usrShowFreightCost> { }
        #endregion

        #region UsrFreightPrice

        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Freight Price")]
        public decimal? UsrFreightPrice { get; set; }
        public abstract class usrFreightPrice : PX.Data.BQL.BqlDecimal.Field<usrFreightPrice> { }

        #endregion

        #region UsrShowFreightPrice 
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public bool? UsrShowFreightPrice { get; set; }
        public abstract class usrShowFreightPrice : PX.Data.BQL.BqlBool.Field<usrShowFreightPrice> { }
        #endregion

        #region UsrShippingInstructions
        [PXDBString(500, IsUnicode = true)]
        [PXUIField(DisplayName = "Shipping Instructions")]
        public string UsrShippingInstructions { get; set; }
        public abstract class usrShippingInstructions : PX.Data.BQL.BqlString.Field<usrShippingInstructions> { }
        #endregion
    }
}
