using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.Common.Bql;
using PX.Objects.Common.Discount.Attributes;
using PX.Objects.Common.Discount;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.DR;
using PX.Objects.GL;
using PX.Objects.IN.Attributes;
using PX.Objects.IN.Matrix.Interfaces;
using PX.Objects.IN.RelatedItems;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.SO.DAC.Projections;
using PX.Objects.SO;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System;

namespace PX.Objects.SO
{
  public class STSOLineExt : PXCacheExtension<PX.Objects.SO.SOLine>
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
  }
}