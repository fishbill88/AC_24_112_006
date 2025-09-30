using CompiledVersion.DAC;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.Update.ExchangeService;
using PX.Objects.AR;
using PX.Objects.SO;
using PX.Objects.IN;
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
    }
}
