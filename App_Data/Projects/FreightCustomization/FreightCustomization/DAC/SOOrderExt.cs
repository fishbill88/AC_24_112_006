using PX.Data;
using PX.Objects.SO;
using System;

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