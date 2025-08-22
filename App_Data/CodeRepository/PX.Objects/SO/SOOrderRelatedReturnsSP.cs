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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO.DAC.Projections;
using PX.Objects.SO.DAC.Unbound;

namespace PX.Objects.SO
{
	[TableAndChartDashboardType]
	public class SOOrderRelatedReturnsSP : PXGraph<SOOrderRelatedReturnsSP>
	{
		private const string EmptyKey = "~";

		public PXCancel<SOOrderRelatedReturnsSPFilter> Cancel;
		public PXFilter<SOOrderRelatedReturnsSPFilter> Filter;

		public SelectFrom<SOOrderRelatedReturnsSPResultDoc>.View.ReadOnly RelatedReturnDocuments;

		public SelectFrom<SOOrderRelatedReturnsSPResultLine>.View.ReadOnly ReturnsByLine;

		[PXHidden]
		[PXCopyPasteHiddenView]
		public SelectFrom<SOReturnShipped>.View.ReadOnly ShippedReturns;

		protected virtual IEnumerable filter()
		{
			if (Filter.Current != null)
			{
				if (string.IsNullOrEmpty(Filter.Current.OrderType) || string.IsNullOrEmpty(Filter.Current.OrderNbr))
				{
					Filter.Current.ShippedQty = 0m;
					Filter.Current.ReturnedQty = 0m;
				}
				else
				{
					SOLine aggrLine = SelectFrom<SOLine>
						.Where<
							SOLine.orderType.IsEqual<SOOrderRelatedReturnsSPFilter.orderType.FromCurrent>
							.And<SOLine.orderNbr.IsEqual<SOOrderRelatedReturnsSPFilter.orderNbr.FromCurrent>>
							.And<SOLine.operation.IsEqual<SOOperation.issue>>>
						.AggregateTo<Sum<SOLine.shippedQty>, Sum<SOLine.baseShippedQty>>
						.View.ReadOnly.SelectSingleBound(this, null);
					Filter.Current.ShippedQty = aggrLine?.LineSign * aggrLine?.BaseShippedQty ?? 0m;
					Filter.Current.ReturnedQty = GetReturnsByLine().Sum(l => l.ReturnedQty) ?? 0m;
				}
			}
			yield return Filter.Current;
			Filter.Cache.IsDirty = false;
		}

		protected virtual IEnumerable relatedReturnDocuments()
		{
			if (RelatedReturnDocuments.Cache.Cached.Any_())
			{
				return RelatedReturnDocuments.Cache.Cached.Cast<SOOrderRelatedReturnsSPResultDoc>();
			}
			int cnt = 0;
			var returnsByLine = GetReturnsByLine().ToArray();
			if (returnsByLine.Any(rl => rl.ReturnOrderNbr != EmptyKey))
			{
				var soReturns = ShippedReturns.Select().RowCast<SOReturnShipped>().ToArray();
				foreach (SOReturnShipped returnShipped in soReturns)
				{
					if (IsDuplicatedMiscLinesOnly(returnShipped, soReturns, returnsByLine)) continue;

					bool directInvoiceNotReleased = returnShipped.ShippingRefNoteID == null && returnShipped.ARNoteID != null;
					RelatedReturnDocuments.Cache.Hold(new SOOrderRelatedReturnsSPResultDoc
					{
						GridLineNbr = cnt++,
						OrderType = returnShipped.OrigOrderType,
						OrderNbr = returnShipped.OrigOrderNbr,
						ReturnOrderType = returnShipped.OrderType,
						ReturnOrderNbr = returnShipped.OrderNbr,
						ShipmentType = directInvoiceNotReleased ? SOShipmentType.Invoice : returnShipped.ShipmentType,
						ShipmentNbr = directInvoiceNotReleased ? Constants.NoShipmentNbr : returnShipped.ShipmentNbr,
						ARDocType = directInvoiceNotReleased ? returnShipped.ARDocType : returnShipped.InvoiceType,
						ARRefNbr = directInvoiceNotReleased ? returnShipped.ARRefNbr : returnShipped.InvoiceNbr,
						APDocType = returnShipped.APDocType,
						APRefNbr = returnShipped.APRefNbr,
					});
				}
			}
			foreach (var g in returnsByLine.Where(rl => rl.ReturnInvoiceNbr != EmptyKey)
				.GroupBy(rl => new { rl.ReturnInvoiceType, rl.ReturnInvoiceNbr }))
			{
				RelatedReturnDocuments.Cache.Hold(new SOOrderRelatedReturnsSPResultDoc
				{
					GridLineNbr = cnt++,
					OrderType = g.First().OrderType,
					OrderNbr = g.First().OrderNbr,
					ReturnInvoiceType = g.Key.ReturnInvoiceType,
					ReturnInvoiceNbr = g.Key.ReturnInvoiceNbr,
					ARDocType = g.Key.ReturnInvoiceType,
					ARRefNbr = g.Key.ReturnInvoiceNbr,
				});
			}
			return RelatedReturnDocuments.Cache.Cached.Cast<SOOrderRelatedReturnsSPResultDoc>();
		}

		protected virtual IEnumerable returnsByLine() => GetReturnsByLine();

		protected virtual IEnumerable<SOOrderRelatedReturnsSPResultLine> GetReturnsByLine()
		{
			if (ReturnsByLine.Cache.Cached.Any_())
			{
				return ReturnsByLine.Cache.Cached.Cast<SOOrderRelatedReturnsSPResultLine>();
			}
			if (string.IsNullOrEmpty(Filter.Current.OrderType) || string.IsNullOrEmpty(Filter.Current.OrderNbr))
			{
				return Array<SOOrderRelatedReturnsSPResultLine>.Empty;
			}
			foreach (SOLine returnLine in SelectFrom<SOLine>
				.Where<
					SOLine.origOrderType.IsEqual<SOOrderRelatedReturnsSPFilter.orderType.FromCurrent>
					.And<SOLine.origOrderNbr.IsEqual<SOOrderRelatedReturnsSPFilter.orderNbr.FromCurrent>>
					.And<SOLine.operation.IsEqual<SOOperation.receipt>>
					.And<SOLine.cancelled.IsEqual<False>>
					.And<SOLine.orderQty.IsNotEqual<decimal0>>>
				.View.ReadOnly.Select(this))
			{
				var resultLine = new SOOrderRelatedReturnsSPResultLine
				{
					OrderType = returnLine.OrigOrderType,
					OrderNbr = returnLine.OrigOrderNbr,
					LineNbr = returnLine.OrigLineNbr,
					ReturnOrderType = returnLine.OrderType,
					ReturnOrderNbr = returnLine.OrderNbr,
					ReturnLineType = returnLine.LineType,
					DisplayReturnOrderType = returnLine.OrderType,
					DisplayReturnOrderNbr = returnLine.OrderNbr,
					ReturnInvoiceType = EmptyKey,
					ReturnInvoiceNbr = EmptyKey,
				};
				var locatedResultLine = ReturnsByLine.Locate(resultLine);
				if (locatedResultLine == null)
				{
					resultLine.InventoryID = returnLine.InventoryID;
					resultLine.BaseUnit = InventoryItem.PK.Find(this, returnLine.InventoryID).BaseUnit;
					resultLine.ReturnedQty = 0m;
					ReturnsByLine.Cache.Hold(resultLine);
				}
				else
				{
					resultLine = locatedResultLine;
				}
				resultLine.ReturnedQty += returnLine.InvtMult *
					(returnLine.LineType != SOLineType.MiscCharge && returnLine.RequireShipping == true && returnLine.Completed == true
						? returnLine.BaseShippedQty : returnLine.BaseOrderQty);
			}
			foreach (PXResult<ARTran, SOLineInvoiced> res in SelectFrom<ARTran>
				.InnerJoin<SOLineInvoiced>.On<SOLineInvoiced.tranType.IsEqual<ARTran.origInvoiceType>
					.And<SOLineInvoiced.refNbr.IsEqual<ARTran.origInvoiceNbr>
					.And<SOLineInvoiced.lineNbr.IsEqual<ARTran.origInvoiceLineNbr>>>>
				.Where<
					SOLineInvoiced.sOOrderType.IsEqual<SOOrderRelatedReturnsSPFilter.orderType.FromCurrent>
					.And<SOLineInvoiced.sOOrderNbr.IsEqual<SOOrderRelatedReturnsSPFilter.orderNbr.FromCurrent>>
					.And<ARTran.sOOrderType.IsNull>
					.And<ARTran.sOOrderNbr.IsNull>
					.And<ARTran.qty.Multiply<ARTran.invtMult>.IsGreater<decimal0>>>
				.View.ReadOnly.Select(this))
			{
				var returnTran = (ARTran)res;
				var origLine = (SOLineInvoiced)res;
				var resultLine = new SOOrderRelatedReturnsSPResultLine
				{
					OrderType = origLine.SOOrderType,
					OrderNbr = origLine.SOOrderNbr,
					LineNbr = origLine.SOOrderLineNbr,
					ReturnOrderType = EmptyKey,
					ReturnOrderNbr = EmptyKey,
					ReturnInvoiceType = returnTran.TranType,
					ReturnInvoiceNbr = returnTran.RefNbr,
					DisplayReturnInvoiceType = returnTran.TranType,
					DisplayReturnInvoiceNbr = returnTran.RefNbr,
				};
				var locatedResultLine = ReturnsByLine.Locate(resultLine);
				if (locatedResultLine == null)
				{
					resultLine.InventoryID = returnTran.InventoryID;
					resultLine.BaseUnit = InventoryItem.PK.Find(this, returnTran.InventoryID).BaseUnit;
					resultLine.ReturnedQty = 0m;
					ReturnsByLine.Cache.Hold(resultLine);
				}
				else
				{
					resultLine = locatedResultLine;
				}
				resultLine.ReturnedQty += returnTran.InvtMult * returnTran.BaseQty;
			}
			return ReturnsByLine.Cache.Cached.Cast<SOOrderRelatedReturnsSPResultLine>();
		}

		protected virtual void _(Events.RowUpdated<SOOrderRelatedReturnsSPFilter> e) => ClearCaches();

		protected virtual void _(Events.FieldUpdated<SOOrderRelatedReturnsSPFilter.orderType> e) => ClearCaches();

		protected virtual void _(Events.FieldUpdated<SOOrderRelatedReturnsSPFilter.orderNbr> e) => ClearCaches();

		private void ClearCaches()
		{
			RelatedReturnDocuments.Cache.Clear();
			RelatedReturnDocuments.View.Clear();
			ReturnsByLine.Cache.Clear();
			ReturnsByLine.View.Clear();
			ShippedReturns.Cache.Clear();
			ShippedReturns.View.Clear();
		}

		private bool IsDuplicatedMiscLinesOnly(SOReturnShipped returnShipped, SOReturnShipped[] soReturns, SOOrderRelatedReturnsSPResultLine[] returnsByLine)
		{
			Lazy<bool> miscLineExists = Lazy.By(()
				=> returnsByLine.Any(l
					=> l.ReturnOrderType == returnShipped.OrderType
					&& l.ReturnOrderNbr == returnShipped.OrderNbr
					&& l.ReturnLineType == SOLineType.MiscCharge));

			if (!string.IsNullOrEmpty(returnShipped.OrderNbr) && returnShipped.ShippingRefNoteID == null
				&& soReturns.Any(r
					=> r.OrderType == returnShipped.OrderType
					&& r.OrderNbr == returnShipped.OrderNbr
					&& r.ShippingRefNoteID != null
					&& string.IsNullOrEmpty(r.InvoiceNbr)))
			{
				if (miscLineExists.Value)
				{
					SOLine nonShippedLine = SelectFrom<SOLine>
						.LeftJoin<SOShipLine>.On<SOShipLine.FK.OrderLine>
						.LeftJoin<POReceiptLine>.On<POReceiptLine.FK.SOLine>
						.Where<SOLine.orderType.IsEqual<SOReturnShipped.orderType.FromCurrent>
							.And<SOLine.orderNbr.IsEqual<SOReturnShipped.orderNbr.FromCurrent>>
							.And<SOLine.origOrderType.IsEqual<SOReturnShipped.origOrderType.FromCurrent>>
							.And<SOLine.origOrderNbr.IsEqual<SOReturnShipped.origOrderNbr.FromCurrent>>
							.And<SOLine.lineType.IsNotEqual<SOLineType.miscCharge>>
							.And<SOShipLine.shipmentNbr.IsNull>
							.And<POReceiptLine.receiptNbr.IsNull>>
						.View.ReadOnly.SelectSingleBound(this, new[] { returnShipped });
					// if shipment is created and the only lines not included to shipment
					// are Misc Charges then don't show them as a separate line
					if (nonShippedLine == null) return true;
				}
			}

			if (!string.IsNullOrEmpty(returnShipped.OrderNbr)
				&& returnShipped.ShipmentType == SOShipmentType.DropShip && !string.IsNullOrEmpty(returnShipped.ShipmentNbr)
				&& string.IsNullOrEmpty(returnShipped.APRefNbr)
				&& soReturns.Any(r
					=> r.OrderType == returnShipped.OrderType
					&& r.OrderNbr == returnShipped.OrderNbr
					&& r.ShipmentType == SOShipmentType.DropShip
					&& r.ShipmentNbr == returnShipped.ShipmentNbr
					&& !string.IsNullOrEmpty(r.APRefNbr)))
			{
				if (miscLineExists.Value)
				{
					SOLine nonBilledLine = SelectFrom<SOLine>
						.InnerJoin<POReceiptLine>.On<POReceiptLine.FK.SOLine>
						.LeftJoin<APTran>.On<APTran.FK.POReceiptLine>
						.Where<SOLine.orderType.IsEqual<SOReturnShipped.orderType.FromCurrent>
							.And<SOLine.orderNbr.IsEqual<SOReturnShipped.orderNbr.FromCurrent>>
							.And<SOLine.origOrderType.IsEqual<SOReturnShipped.origOrderType.FromCurrent>>
							.And<SOLine.origOrderNbr.IsEqual<SOReturnShipped.origOrderNbr.FromCurrent>>
							.And<SOLine.lineType.IsNotEqual<SOLineType.miscCharge>>
							.And<POReceiptLine.receiptType.IsEqual<POReceiptType.poreturn>>
							.And<POReceiptLine.receiptNbr.IsEqual<SOReturnShipped.shipmentNbr.FromCurrent>>
							.And<APTran.refNbr.IsNull>>
						.View.ReadOnly.SelectSingleBound(this, new[] { returnShipped });
					// if po return is created and billed but the only lines not included to po return
					// are Misc Charges then don't show them as a separate line
					if (nonBilledLine == null) return true;
				}
			}
			return false;
		}
	}
}
