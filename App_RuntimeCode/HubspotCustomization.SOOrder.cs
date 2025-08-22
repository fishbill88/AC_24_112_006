using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.WorkflowAPI;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.Common.Attributes;
using PX.Objects.Common.Extensions;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN.RelatedItems;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.SO.Attributes;
using PX.Objects.SO.Interfaces;
using PX.Objects.SO;
using PX.Objects.TX;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace PX.Objects.SO
{
  public class STSOOrderExt : PXCacheExtension<PX.Objects.SO.SOOrder>
  {
    #region UsrShippingInstructions
    [PXDBString(500, IsUnicode = true)]
    [PXUIField(DisplayName="Shipping Instructions")]
    public virtual string UsrShippingInstructions { get; set; }
    public abstract class usrShippingInstructions : PX.Data.BQL.BqlString.Field<usrShippingInstructions> { }
    #endregion
      
      
    #region UsrShippingNotes
    [PXDBString(500, IsUnicode = true)]
    [PXUIField(DisplayName="Internal Shipping Notes")]
    public virtual string UsrShippingNotes { get; set; }
    public abstract class usrShippingNotes : PX.Data.BQL.BqlString.Field<usrShippingNotes> { }
    #endregion
      
    #region UsrCustomerAccount
    [PXDBString(255, IsUnicode = true)]
    [PXUIField(DisplayName="Customer Account")]
    [PXUIVisible(typeof(Where<Current<SOOrder.useCustomerAccount>,Equal<True>>))]
    public virtual string UsrCustomerAccount { get; set; }
    public abstract class usrCustomerAccount : PX.Data.BQL.BqlString.Field<usrCustomerAccount> { }
    #endregion
  }
}