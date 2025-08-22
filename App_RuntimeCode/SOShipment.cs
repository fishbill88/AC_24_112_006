using CRLocation = PX.Objects.CR.Standalone.Location;
using POReceipt = PX.Objects.PO.POReceipt;
using PX.Common;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.WorkflowAPI;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.Common.Attributes;
using PX.Objects.Common.GraphExtensions.Abstract;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO.Attributes;
using PX.Objects.SO;
using PX.Objects;
using PX.SM;
using PX.TM;
using System.Collections.Generic;
using System;

namespace PX.Objects.SO
{
  public class STSOShipmentExt : PXCacheExtension<PX.Objects.SO.SOShipment>
  {
    #region UsrShippingNotes
    [PXDBString(500, IsUnicode = true)]
    [PXUIField(DisplayName="Shipping Notes")]
    public virtual string UsrShippingNotes { get; set; }
    public abstract class usrShippingNotes : PX.Data.BQL.BqlString.Field<usrShippingNotes> { }
    #endregion
  }
}