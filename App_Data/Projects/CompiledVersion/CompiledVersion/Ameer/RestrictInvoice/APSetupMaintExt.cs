using PX.Data;
using PX.Objects.AP;

namespace RestrictInvoice
{
    /// <summary>
    /// Extension for APSetupMaint graph to support PO-Bill restriction settings
    /// </summary>
    public class APSetupMaintExt : PXGraphExtension<APSetupMaint>
    {
        #region Event Handlers

        protected virtual void _(Events.RowSelected<APSetup> e)
        {
            if (e.Row == null) return;

            APSetup setup = e.Row;
            APSetupExt setupExt = PXCache<APSetup>.GetExtension<APSetupExt>(setup);

            // Enable/disable amount tolerance based on restriction checkbox
            PXUIFieldAttribute.SetEnabled<APSetupExt.usrPOBillAmountTolerance>(
                e.Cache, 
                setup, 
                setupExt.UsrEnablePOBillRestriction == true);
        }

        #endregion
    }
}
