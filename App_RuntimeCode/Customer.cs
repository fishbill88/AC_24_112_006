using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CR.MassProcess;
using PX.Objects.CR;
using PX.Objects.CS.DAC;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL.DAC;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace PX.Objects.AR
{
  public class STCustomerExt: PXCacheExtension<Customer>
  {
    #region UsrShippingInstructions
    [PXDBString(500, IsUnicode = true)]
    [PXUIField(DisplayName="Shipping Instructions")]
    public virtual string UsrShippingInstructions { get; set; }
    public abstract class usrShippingInstructions : PX.Data.BQL.BqlString.Field<usrShippingInstructions> { }
    #endregion
  }
}