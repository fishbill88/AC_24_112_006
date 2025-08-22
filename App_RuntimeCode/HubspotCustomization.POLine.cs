using PX.Common;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM.Extensions;
using PX.Objects.Common.Discount.Attributes;
using PX.Objects.Common.Discount;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN.Attributes;
using PX.Objects.IN.Matrix.Interfaces;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.SO;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System;

namespace PX.Objects.PO
{
  public class STPOLineExt : PXCacheExtension<PX.Objects.PO.POLine>
  {
    #region UsrVendorSpecTerms
    [PXDBString(250, IsUnicode=true)]
    [PXUIField(DisplayName="Vendor Special Terms")]
    public virtual string UsrVendorSpecTerms { get; set; }
    public abstract class usrVendorSpecTerms : PX.Data.BQL.BqlString.Field<usrVendorSpecTerms> { }
    #endregion
    
    #region UsrVendorNotes
    [PXDBString(500, IsUnicode=true)]
    [PXUIField(DisplayName="Vendor Notes")]
    public virtual string UsrVendorNotes { get; set; }
    public abstract class usrVendorNotes : PX.Data.BQL.BqlString.Field<usrVendorNotes> { }
    #endregion
      
    #region UsrShippingTerms
    [PXString(1, IsFixed=true)]
    [SOShipComplete.List()]
    [PXUIField(DisplayName="Shipping Rule", Enabled = false)]
    public virtual string UsrShippingTerms { get; set; }
    public abstract class usrShippingTerms : PX.Data.BQL.BqlString.Field<usrShippingTerms> { }
    #endregion
      
    #region UsrSkipPrint
    [PXDBBool]
    [PXUIField(DisplayName = "Skip Print")]
    public bool? UsrSkipPrint { get; set; }
    public abstract class usrSkipPrint : PX.Data.BQL.BqlBool.Field<usrSkipPrint> { }
    #endregion

    #region UsrPrepaymentLine
    [PXDBBool]
    [PXUIField(DisplayName = "Prepayment Line")]
    public bool? UsrPrepaymentLine { get; set; }
    public abstract class usrPrepaymentLine: PX.Data.BQL.BqlBool.Field<usrPrepaymentLine> { }
    #endregion
  }
}