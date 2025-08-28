using PX.Data;
using PX.Objects.CS;
using PX.Objects.PO;
using System;

namespace CompiledVersion.DAC
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

        #region UsrShipTermsID
        [PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXUIField(DisplayName = "Shipping Terms")]
        [PXSelector(typeof(ShipTerms.shipTermsID), DescriptionField = typeof(ShipTerms.description), CacheGlobal = true)]
        public string UsrShipTermsID { get; set; }
        public abstract class usrShipTermsID : PX.Data.BQL.BqlString.Field<usrShipTermsID> { }
        #endregion

        #region UsrCustomerAccount
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Carrier Account")]
        public string UsrCustomerAccount { get; set; }
        public abstract class usrCustomerAccount : PX.Data.BQL.BqlString.Field<usrCustomerAccount> { }
        #endregion

        #region UsrRTHDetailTotal
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "RTH Detail Total", Enabled = false)]
        public decimal? UsrRTHDetailTotal { get; set; }
        public abstract class usrRTHDetailTotal : PX.Data.BQL.BqlDecimal.Field<usrRTHDetailTotal> { }
        #endregion

        #region UsrRTHLineDiscount
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "RTH Line Discount", Enabled = false)]
        public decimal? UsrRTHLineDiscount { get; set; }
        public abstract class usrRTHLineDiscount : PX.Data.BQL.BqlDecimal.Field<usrRTHLineDiscount> { }
        #endregion

        #region UsrRTHDocDiscount
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "RTH Doc. Discount", Enabled = false)]
        public decimal? UsrRTHDocDiscount { get; set; }
        public abstract class usrRTHDocDiscount : PX.Data.BQL.BqlDecimal.Field<usrRTHDocDiscount> { }
        #endregion

        #region UsrRTHTaxTotal
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "RTH Tax Total", Enabled = false)]
        public decimal? UsrRTHTaxTotal { get; set; }
        public abstract class usrRTHTaxTotal : PX.Data.BQL.BqlDecimal.Field<usrRTHTaxTotal> { }
        #endregion

        #region UsrRTHOrderTotal
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "RTH Order Total", Enabled = false)]
        public decimal? UsrRTHOrderTotal { get; set; }
        public abstract class usrRTHOrderTotal : PX.Data.BQL.BqlDecimal.Field<usrRTHOrderTotal> { }
        #endregion

        #region UsrCustomerOrderNbr
        [PXDBString(40, IsUnicode = true)]
        [PXUIField(DisplayName = "Customer PO Nbr")]
        public string UsrCustomerOrderNbr { get; set; }
        public abstract class usrCustomerOrderNbr : PX.Data.BQL.BqlString.Field<usrCustomerOrderNbr> { }
        #endregion
    }
}
