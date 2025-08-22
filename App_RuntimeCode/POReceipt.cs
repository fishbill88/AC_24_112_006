using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Data.BQL.Fluent;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.WorkflowAPI;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CM.Extensions;
using PX.Objects.Common.Bql;
using PX.Objects.Common.GraphExtensions.Abstract;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PX.Objects.PO
{
  public class STPOReceiptExt : PXCacheExtension<PX.Objects.PO.POReceipt>
  {
    
    #region UsrFOBPoint
    [PXDBString(15, IsUnicode = true)]
    [PXUIField(DisplayName = "FOB Point", Enabled = false)]
    [PXSelector(typeof(Search<FOBPoint.fOBPointID>), DescriptionField = typeof(FOBPoint.description), CacheGlobal = true)]
    public virtual string UsrFOBPoint { get; set; }
    public abstract class usrFOBPoint : PX.Data.BQL.BqlString.Field<usrFOBPoint> { }
    #endregion
      
    #region UsrShipVia
    [PXDBString(15, IsUnicode = true)]
    [PXUIField(DisplayName = "Ship Via", Enabled = false)]
    [PXSelector(typeof(Search<Carrier.carrierID>), CacheGlobal = true)]
    public virtual string UsrShipVia { get; set; }
    public abstract class usrShipVia : PX.Data.BQL.BqlString.Field<usrShipVia> { }
    #endregion 
  
    #region UsrShipTermsID
    [PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
    [PXUIField(DisplayName = "Shipping Terms", Enabled = false)]
    [PXSelector(typeof(ShipTerms.shipTermsID), DescriptionField = typeof(ShipTerms.description), CacheGlobal = true)]
    public virtual string UsrShipTermsID { get; set; }
    public abstract class usrShipTermsID : PX.Data.BQL.BqlString.Field<usrShipTermsID> { }
    #endregion
      
    #region UsrFreightCost
    [PXDBDecimal(4)]
    [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
    [PXUIField(DisplayName = "Freight Cost")]
    public virtual Decimal? UsrFreightCost { get; set; }
    public abstract class usrFreightCost : PX.Data.BQL.BqlDecimal.Field<usrFreightCost> { }
    #endregion
      
    #region UsrFreightPrice
    [PXDBDecimal(4)]
    [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
    [PXUIField(DisplayName = "Freight Price")]
    public virtual Decimal? UsrFreightPrice { get; set; }
    public abstract class usrFreightPrice : PX.Data.BQL.BqlDecimal.Field<usrFreightPrice> { }
    #endregion 
      
    #region UsrCarrierAccount
    [PXDBString(255, IsUnicode = true)]
    [PXUIField(DisplayName="Carrier Account", Enabled = false)]
    public virtual string UsrCarrierAccount { get; set; }
    public abstract class usrCarrierAccount : PX.Data.BQL.BqlString.Field<usrCarrierAccount> { }
    #endregion
      
    #region UsrTrackingNumber
    [PXDBString(100, IsUnicode = true)]
    [PXUIField(DisplayName="Tracking Number")]
    public virtual string UsrTrackingNumber { get; set; }
    public abstract class usrTrackingNumber : PX.Data.BQL.BqlString.Field<usrTrackingNumber> { }
    #endregion  
      
    
      
    #region UsrHasShippingTab
    [PXDBBool]
    [PXUIField(DisplayName="From Drop-Ship", Visible=false)]
    public virtual bool? UsrHasShippingTab { get; set; }
    public abstract class usrHasShippingTab : PX.Data.BQL.BqlBool.Field<usrHasShippingTab> { }
    #endregion  
  }
}