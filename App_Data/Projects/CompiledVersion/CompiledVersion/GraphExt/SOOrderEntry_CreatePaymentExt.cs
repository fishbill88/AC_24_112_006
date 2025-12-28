using CompiledVersion.DAC;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.SO;
using PX.Objects.SO.GraphExtensions.SOOrderEntryExt;

namespace CompiledVersion.Graphs
{
    public class SOOrderEntry_CreatePaymentExt : PXGraphExtension<CreatePaymentExt, PX.Objects.SO.SOOrderEntry>
    {
        public static bool IsActive() => true;

        #region Event Handlers - Override to use RTH Order Total

        protected virtual void _(Events.FieldDefaulting<SOQuickPayment, SOQuickPayment.curyOrigDocAmt> e, PXFieldDefaulting baseHandler)
        {
            // Call base handler first
            baseHandler?.Invoke(e.Cache, e.Args);
            
            // Override with RTH Order Total
            decimal? rthAmount = GetRTHDefaultPaymentAmount(e.Row as SOQuickPayment);
            if (rthAmount != null)
            {
                e.NewValue = rthAmount;
            }
        }

        protected virtual void _(Events.FieldDefaulting<SOQuickPayment, SOQuickPayment.curyRefundAmt> e, PXFieldDefaulting baseHandler)
        {
            // Call base handler first
            baseHandler?.Invoke(e.Cache, e.Args);
            
            // Override with RTH Order Total
            decimal? rthAmount = GetRTHDefaultPaymentAmount(e.Row as SOQuickPayment);
            if (rthAmount != null)
            {
                e.NewValue = rthAmount;
            }
        }

        protected virtual decimal? GetRTHDefaultPaymentAmount(SOQuickPayment qp)
        {
            if (qp == null || Base.Document.Current == null)
                return null;

            SOOrder document = Base.Document.Current;
            SOOrderExt documentExt = document.GetExtension<SOOrderExt>();

            // Use RTH Order Total instead of standard order total
            decimal? rthOrderTotal = documentExt?.UsrRTHCuryOrderTotal;
            if (rthOrderTotal == null || rthOrderTotal <= 0)
            {
                // Return null to use base calculation
                return null;
            }

            decimal? amt = null;

            if (qp.CuryID == document.CuryID)
            {
                // Calculate unpaid balance using RTH Order Total
                decimal? paidAmount = document.CuryPaymentTotal ?? 0m;
                amt = rthOrderTotal - paidAmount;
            }
            else if (qp.CuryID != null)
            {
                // Convert the unpaid balance based on RTH Order Total
                decimal? paidAmount = document.PaymentTotal ?? 0m;
                decimal baseUnpaidBalance = (rthOrderTotal ?? 0m) - (paidAmount ?? 0m);
                PXCurrencyAttribute.CuryConvCury(Base1.QuickPayment.Cache, qp, baseUnpaidBalance, out decimal curyUnpaidBalance);
                amt = curyUnpaidBalance;
            }

            return amt;
        }

        #endregion
    }
}
