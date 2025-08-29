using CompiledVersion.DAC;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.Update.ExchangeService;
using PX.Objects.AR;
using PX.Objects.SO;
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
    }
}
