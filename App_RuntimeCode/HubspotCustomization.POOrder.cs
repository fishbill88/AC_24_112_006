using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.WorkflowAPI;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM.Extensions;
using PX.Objects.CN.Subcontracts.SC.Graphs;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System;

namespace PX.Objects.PO
{
  public class STPOOrderExt : PXCacheExtension<PX.Objects.PO.POOrder>
  {
    #region UsrShipTermsID
    [PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
    [PXUIField(DisplayName = "Shipping Terms")]
    [PXSelector(typeof(ShipTerms.shipTermsID), DescriptionField = typeof(ShipTerms.description), CacheGlobal = true)]
    public virtual string UsrShipTermsID { get; set; }
    public abstract class usrShipTermsID : PX.Data.BQL.BqlString.Field<usrShipTermsID> { }
    #endregion
      
    #region UsrCustomerAccount
    [PXDBString(255, IsUnicode = true)]
    [PXUIField(DisplayName="Carrier Account")]
    public virtual string UsrCustomerAccount { get; set; }
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
  }
}