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
using PX.Objects.CR;
using PX.Objects.TX;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using PX.Objects.GDPR;

namespace FreightCustomization
{
    public sealed class SOOrderExt : PXCacheExtension<SOOrder>
    {
        
        public static bool IsActive() => true;
        #region UsrFreightPriceLimit
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Freight Limit")]
        [PXUIVisible(typeof(Where<Current<SOOrder.shipTermsID>, Equal<Current<SOSetupExt.usrNotToExceed>>>))]
        public decimal? UsrFreightPriceLimit { get; set; }
        public abstract class usrFreightPriceLimit : PX.Data.BQL.BqlDecimal.Field<usrFreightPriceLimit> { }
        #endregion

        #region UsrFreightTotal
        [PXDecimal(2)]
        [PXUIField(DisplayName = "Freight Total", Enabled = false)]
        public decimal? UsrFreightTotal { get; set; }
        public abstract class usrFreightTotal : PX.Data.BQL.BqlDecimal.Field<usrFreightTotal> { }
        #endregion

        #region UsrShippingInstructions
        [PXDBString(500, IsUnicode = true)]
        [PXUIField(DisplayName = "Shipping Instructions")]
        public string UsrShippingInstructions { get; set; }
        public abstract class usrShippingInstructions : PX.Data.BQL.BqlString.Field<usrShippingInstructions> { }
        #endregion
    }
}