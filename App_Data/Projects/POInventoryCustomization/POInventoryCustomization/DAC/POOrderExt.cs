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

namespace POInventoryCustomization
{
    public class POOrderExt : PXCacheExtension<PX.Objects.PO.POOrder>
    {
        public static bool IsActive()
        {
            // This method is used to determine if the extension is active.
            // You can add your custom logic here to enable or disable the extension.
            return true;
        }
        #region UsrCustomerOrderNbr
        [PXDBString(40, IsUnicode = true)]
        [PXUIField(DisplayName = "Customer PO Nbr")]
        public virtual string UsrCustomerOrderNbr { get; set; }
        public abstract class usrCustomerOrderNbr : PX.Data.BQL.BqlString.Field<usrCustomerOrderNbr> { }
        #endregion
    }
}