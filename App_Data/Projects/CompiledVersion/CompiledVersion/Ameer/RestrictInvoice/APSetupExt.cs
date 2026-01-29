using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using System;

namespace RestrictInvoice
{
    /// <summary>
    /// Extension for APSetup to add PO-Bill restriction settings
    /// </summary>
    public sealed class APSetupExt : PXCacheExtension<APSetup>
    {
        #region UsrEnablePOBillRestriction
        [PXDBBool]
        [PXUIField(DisplayName = "Enable PO-Bill Restriction")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public bool? UsrEnablePOBillRestriction { get; set; }
        public abstract class usrEnablePOBillRestriction : PX.Data.BQL.BqlBool.Field<usrEnablePOBillRestriction> { }
        #endregion

        #region UsrPOBillAmountTolerance
        [PXDBDecimal(2, MinValue = 0.00, MaxValue = 999.99)]
        [PXUIField(DisplayName = "Amount Tolerance")]
        [PXDefault(TypeCode.Decimal, "0.01", PersistingCheck = PXPersistingCheck.Nothing)]
        public decimal? UsrPOBillAmountTolerance { get; set; }
        public abstract class usrPOBillAmountTolerance : PX.Data.BQL.BqlDecimal.Field<usrPOBillAmountTolerance> { }
        #endregion

        public static class Messages
        {
            public const string EnablePOBillRestrictionDescription = "When enabled, enforces 1 PO = 1 Bill = 1 Receipt relationship";
            public const string AmountToleranceDescription = "Maximum allowed difference between Bill and PO amounts (default: 0.01)";
        }
    }
}
