using CompiledVersion.DAC;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.Update.ExchangeService;
using PX.Objects.AR;
using PX.Objects.SO;
using PX.Objects.IN;
using POInventoryCustomization; // For SOOrderExt with UsrRTHCuryFreightTot
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledVersion.Graphs
{
    public class SOInvoiceEntry_Extension : PXGraphExtension<SOInvoiceEntry>
    {
        public static bool IsActive() => true;

        public SelectFrom<SOCustSalesPeople>.View SalesPeople;

        public IEnumerable salesPeople()
        {
            var invoice = Base.Document.Current;
            if (invoice == null)
                yield break;
            foreach (SOFreightDetail fd in PXSelect<SOFreightDetail, 
                                    Where<SOFreightDetail.docType, Equal<Current<ARInvoice.docType>>, 
                                        And<SOFreightDetail.refNbr, Equal<Current<ARInvoice.refNbr>>>>>.Select(Base))
            {
                foreach (SOCustSalesPeople sp in SelectFrom<SOCustSalesPeople>.Where<SOCustSalesPeople.orderType.IsEqual<@P.AsString>
                                    .And<SOCustSalesPeople.orderNbr.IsEqual<@P.AsString>>>.View.Select(Base, fd.OrderType,fd.OrderNbr))
                {
                    yield return sp;
                }

            }
        }

        protected void _(Events.RowSelected<SOCustSalesPeople> e)
        {
            if (e.Row == null) return;

            PXUIFieldAttribute.SetEnabled<SOCustSalesPeople.commisionPct>(e.Cache, e.Row, false);
        }

        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            foreach (ARTran item in Base.Transactions.Select())
            {
                if (TryGetNonStockError(item.RefNbr, out string errorMessage))
                {
                    PXUIFieldAttribute.SetError<ARTran.inventoryID>(Base.Transactions.Cache, item, errorMessage);
                    throw new PXException(errorMessage);
                }
            }

            baseMethod();
        }
        private bool TryGetNonStockError(string refNbr, out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrEmpty(refNbr))
                return false;

            var setupExt = Base.sosetup.Current.GetExtension<SOSetupExt>();
            if (setupExt == null) return false;

            var nonStockItems = new List<int?>();
            if (setupExt.UsrNonstock1 != null) nonStockItems.Add(setupExt.UsrNonstock1);
            if (setupExt.UsrNonstock2 != null) nonStockItems.Add(setupExt.UsrNonstock2);
            if (setupExt.UsrNonstock3 != null) nonStockItems.Add(setupExt.UsrNonstock3);
            if (nonStockItems.Count == 0)
                return false;

            var lines = PXSelect<ARTran,
                Where<ARTran.refNbr, Equal<Required<ARTran.refNbr>>>>
                .Select(Base, refNbr)
                .RowCast<ARTran>();

            var invalidNonstock = new List<string>();

            foreach (ARTran line in (lines.ToList().Count > 0 ? lines.ToList() : Base.Transactions.Select().RowCast<ARTran>().ToList()))
            {
                if (nonStockItems.Contains(line.InventoryID))
                {
                    InventoryItem item = InventoryItem.PK.Find(Base, line.InventoryID);

                    if (item != null)
                        invalidNonstock.Add(item.InventoryCD);

                    // Mark the line field so if user opens the shipment they see the problematic lines
                    PXUIFieldAttribute.SetError<ARTran.inventoryID>(Base.Transactions.Cache, line,
                        string.Format("You cannot invoice this non-stock item for {0}.", refNbr));
                }
            }

            if (invalidNonstock.Count > 0)
            {
                errorMessage = Messages.CannotInvoiceNonStockItems(string.Join(", ", invalidNonstock));
                return true;
            }

            return false;
        }

        #region Freight Override - Use SOOrder.UsrRTHCuryFreightTot for Invoice Freight

        /// <summary>
        /// Override ARInvoice freight total right before persisting to database.
        /// This ensures we use SOOrder.UsrRTHCuryFreightTot instead of the calculated freight
        /// from SOFreightDetail records. Also updates the SOFreightDetail records to match.
        /// 
        /// Business Rules (as of 2025-12-17):
        /// - Freight is not taxed
        /// - Only one open shipment per sales order is allowed
        /// - Freight should be reversed for returns/credit memos
        /// 
        /// TODO: FUTURE ENHANCEMENT - Add order type filtering if business decides 
        /// this should only apply to specific order types instead of all types.
        /// </summary>
        protected virtual void _(Events.RowPersisting<ARInvoice> e)
        {
            if (e.Row == null || e.Operation == PXDBOperation.Delete)
                return;

            ARInvoice invoice = e.Row;

            // Find all freight details for this invoice
            var freightDetails = PXSelect<SOFreightDetail,
                Where<SOFreightDetail.docType, Equal<Required<ARInvoice.docType>>,
                    And<SOFreightDetail.refNbr, Equal<Required<ARInvoice.refNbr>>>>>
                .Select(Base, invoice.DocType, invoice.RefNbr);

            if (freightDetails.Count == 0) return;

            // Get the first detail to find the order
            SOFreightDetail firstDetail = freightDetails.FirstOrDefault();
            if (firstDetail == null) return;
            
            // Get the sales order
            SOOrder order = SOOrder.PK.Find(Base, firstDetail.OrderType, firstDetail.OrderNbr);
            if (order == null) return;

            // Get the custom freight total from the order extension
            var orderExt = order.GetExtension<SOOrderExt>();
            if (orderExt?.UsrRTHCuryFreightTot == null)
                return;

            // Apply the custom freight amount
            decimal targetFreightTotal = orderExt.UsrRTHCuryFreightTot.Value;

            // Update the invoice header
            invoice.CuryFreightTot = targetFreightTotal;

            // Update SOFreightDetail records to match the new total
            // Strategy: Set the first detail to the full amount, zero out the rest
            bool isFirst = true;
            foreach (SOFreightDetail detail in freightDetails)
            {
                if (isFirst)
                {
                    // Put the entire UsrRTHCuryFreightTot into CuryFreightAmt of first detail
                    detail.CuryFreightAmt = targetFreightTotal;
                    detail.CuryPremiumFreightAmt = 0m; // Zero out premium
                    isFirst = false;
                }
                else
                {
                    // Zero out any additional freight detail records
                    detail.CuryFreightAmt = 0m;
                    detail.CuryPremiumFreightAmt = 0m;
                }
                
                Base.FreightDetails.Update(detail);
            }
        }

        #endregion
    }
}
