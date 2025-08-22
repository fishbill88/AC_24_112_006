/* ---------------------------------------------------------------------*
*                             Acumatica Inc.                            *

*              Copyright (c) 2005-2024 All rights reserved.             *

*                                                                       *

*                                                                       *

* This file and its contents are protected by United States and         *

* International copyright laws.  Unauthorized reproduction and/or       *

* distribution of all or any portion of the code contained herein       *

* is strictly prohibited and will result in severe civil and criminal   *

* penalties.  Any violations of this copyright will be prosecuted       *

* to the fullest extent possible under law.                             *

*                                                                       *

* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *

* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *

* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY ACUMATICA PRODUCT.       *

*                                                                       *

* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *

* --------------------------------------------------------------------- */


using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.SO;
using System;

namespace PX.Objects.IN.RelatedItems
{
    public class ReleaseRelatedItemHistory<TGraph> : PXGraphExtension<TGraph>
        where TGraph : PXGraph
    {
        public SelectFrom<RelatedItemHistory>.View HistoryLines;

        protected virtual void ReleaseHistory(ARRegister ardoc)
        {
            ReleaseRelatedItemHistoryFromInvoice(ardoc);
            ReleaseRelatedItemHistoryFromOrder(ardoc);
        }

        protected virtual void ReleaseRelatedItemHistoryFromInvoice(ARRegister ardoc)
        {
            var select = new SelectFrom<ARTran>
                .InnerJoin<RelatedItemHistory>
                    .On<RelatedItemHistory.FK.RelatedInvoiceLine
                    .And<RelatedItemHistory.isDraft.IsEqual<True>>
                    .And<ARTran.sOOrderNbr.IsNull>
                    .And<ARTran.tranType.IsEqual<@P.AsString.ASCII>>
                    .And<ARTran.refNbr.IsEqual<@P.AsString>>>
                .View(Base);

            foreach (PXResult<ARTran, RelatedItemHistory> pair in select.Select(ardoc.DocType, ardoc.RefNbr))
            {
                ReleaseHistoryLine(pair, pair, ardoc.DocDate, false);
            }
        }

        protected virtual void ReleaseRelatedItemHistoryFromOrder(ARRegister ardoc)
        {
            var select = new SelectFrom<ARTran>
                .InnerJoin<SOLine>
                    .On<ARTran.FK.SOOrderLine
                    .And<ARTran.tranType.IsEqual<@P.AsString.ASCII>>
                    .And<ARTran.refNbr.IsEqual<@P.AsString>>>
                .InnerJoin<SOOrder>
                    .On<SOLine.FK.Order>
                .InnerJoin<RelatedItemHistory>
                    .On<RelatedItemHistory.FK.RelatedSalesOrderLine>
                .View(Base);
            using (new PXFieldScope(select.View, typeof(ARTran), typeof(RelatedItemHistory), typeof(SOOrder.orderDate)))
            {
                foreach (PXResult<ARTran, SOLine, SOOrder, RelatedItemHistory> tuple in select.Select(ardoc.DocType, ardoc.RefNbr))
                {
                    RelatedItemHistory historyLine = tuple;
                    historyLine = (RelatedItemHistory)HistoryLines.Cache.Locate(historyLine) ?? historyLine;
                    ReleaseHistoryLine(historyLine, tuple, ((SOOrder)tuple).OrderDate, ardoc.IsCancellation == true);
                }
            }
        }

        protected virtual void ReleaseHistoryLine(RelatedItemHistory historyLine, ARTran tran, DateTime? documentDate, bool revert)
        {
            var soldQty = historyLine.RelatedInventoryUOM == tran.UOM
                ? tran.Qty
                : INUnitAttribute.ConvertFromBase(HistoryLines.Cache, historyLine.RelatedInventoryID, historyLine.RelatedInventoryUOM, tran.BaseQty ?? 0, INPrecision.QUANTITY);
            var newSoldQty = (historyLine.SoldQty ?? 0) + Sign.MinusIf(revert) * soldQty;
            if (newSoldQty <= 0)
            {
                if (historyLine.IsDraft == true)
                    return;
                historyLine.IsDraft = true;
            }
            else
                historyLine.IsDraft = false;

            if(!revert)
                historyLine.DocumentDate = documentDate;

            historyLine.SoldQty = newSoldQty;

            HistoryLines.Update(historyLine);
        }
    }
}
