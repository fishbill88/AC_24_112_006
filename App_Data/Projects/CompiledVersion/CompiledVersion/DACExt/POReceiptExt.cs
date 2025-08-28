using PX.Data;
using PX.Objects.CS;
using System;

namespace CompiledVersion.DAC
{
    public sealed class POReceiptExt : PXCacheExtension<PX.Objects.PO.POReceipt>
    {
        public static bool IsActive() => true;

        #region UsrFOBPoint
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "FOB Point", Enabled = false)]
        [PXSelector(typeof(Search<FOBPoint.fOBPointID>), DescriptionField = typeof(FOBPoint.description), CacheGlobal = true)]
        public string UsrFOBPoint { get; set; }
        public abstract class usrFOBPoint : PX.Data.BQL.BqlString.Field<usrFOBPoint> { }
        #endregion

        #region UsrShipVia
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Ship Via", Enabled = false)]
        [PXSelector(typeof(Search<Carrier.carrierID>), CacheGlobal = true)]
        public string UsrShipVia { get; set; }
        public abstract class usrShipVia : PX.Data.BQL.BqlString.Field<usrShipVia> { }
        #endregion

        #region UsrShipTermsID
        [PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXUIField(DisplayName = "Shipping Terms", Enabled = false)]
        [PXSelector(typeof(ShipTerms.shipTermsID), DescriptionField = typeof(ShipTerms.description), CacheGlobal = true)]
        public string UsrShipTermsID { get; set; }
        public abstract class usrShipTermsID : PX.Data.BQL.BqlString.Field<usrShipTermsID> { }
        #endregion

        #region UsrFreightCost
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Freight Cost")]
        public Decimal? UsrFreightCost { get; set; }
        public abstract class usrFreightCost : PX.Data.BQL.BqlDecimal.Field<usrFreightCost> { }
        #endregion

        #region UsrFreightPrice
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Freight Price")]
        public Decimal? UsrFreightPrice { get; set; }
        public abstract class usrFreightPrice : PX.Data.BQL.BqlDecimal.Field<usrFreightPrice> { }
        #endregion

        #region UsrCarrierAccount
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Carrier Account", Enabled = false)]
        public string UsrCarrierAccount { get; set; }
        public abstract class usrCarrierAccount : PX.Data.BQL.BqlString.Field<usrCarrierAccount> { }
        #endregion

        #region UsrTrackingNumber
        [PXDBString(100, IsUnicode = true)]
        [PXUIField(DisplayName = "Tracking Number")]
        public string UsrTrackingNumber { get; set; }
        public abstract class usrTrackingNumber : PX.Data.BQL.BqlString.Field<usrTrackingNumber> { }
        #endregion



        #region UsrHasShippingTab
        [PXDBBool]
        [PXUIField(DisplayName = "From Drop-Ship", Visible = false)]
        public bool? UsrHasShippingTab { get; set; }
        public abstract class usrHasShippingTab : PX.Data.BQL.BqlBool.Field<usrHasShippingTab> { }
        #endregion
    }
}