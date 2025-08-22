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
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;

using PX.Objects.Common;
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.SO.DAC.Projections;

namespace PX.Objects.SO.GraphExtensions
{
	public abstract class SOBaseItemAvailabilityExtension<TGraph, TLine, TSplit> : IN.GraphExtensions.ItemAvailabilityExtension<TGraph, TLine, TSplit>
		where TGraph : PXGraph
		where TLine : class, IBqlTable, ILSPrimary, new()
		where TSplit : class, IBqlTable, ILSDetail, new()
	{
		public virtual ReturnedQtyResult MemoCheckQty(
			int? inventoryID,
			string arDocType, string arRefNbr, int? arTranLineNbr,
			string orderType, string orderNbr, int? orderLineNbr,
			InvoiceSplit split = null)
		{
			var qtyResult = new ReturnedQtyResult(true);

			bool hasRefToOrigSOLine = orderType != null && orderNbr != null && orderLineNbr != null;
			bool hasRefToOrigARTran = arDocType != null && arRefNbr != null && arTranLineNbr != null;
			if (!hasRefToOrigSOLine && !hasRefToOrigARTran)
				return qtyResult;

			var invoiced = split?.IsKit == false ? split.AsSingleEnumerable() : SelectInvoicedRecords(arDocType, arRefNbr, arTranLineNbr);

			//return SO lines (including current document, excluding cancelled orders):
			var returnSOLines = SelectReturnSOLines(arDocType, arRefNbr, arTranLineNbr);

			//return direct AR Transactions (including current document):
			var returnARTrans = SelectReturnARTrans(arDocType, arRefNbr);

			if (hasRefToOrigSOLine)
			{
				var invoicedFromSOLine = invoiced
					.Where(r =>
						r.SOOrderType == orderType &&
						r.SOOrderNbr == orderNbr &&
						r.SOLineNbr == orderLineNbr);
				var returnedFromSOLine = returnSOLines
					.Where(l =>
						l.OrigOrderType == orderType &&
						l.OrigOrderNbr == orderNbr
						&& l.OrigLineNbr == orderLineNbr)
					.Select(ReturnRecord.FromSOLine);
				qtyResult = CheckInvoicedAndReturnedQty(inventoryID, invoicedFromSOLine, returnedFromSOLine);
			}

			if (qtyResult.Success == true && hasRefToOrigARTran)
			{
				var returnedFromOrigARTran =
						returnARTrans
						.Where(t => t.OrigInvoiceLineNbr == arTranLineNbr)
						.Select(ReturnRecord.FromARTran)
					.Concat(
						returnSOLines
						.Where(l => l.InvoiceLineNbr == arTranLineNbr)
						.Select(ReturnRecord.FromSOLine));
				qtyResult = CheckInvoicedAndReturnedQty(inventoryID, invoiced, returnedFromOrigARTran);
			}

			return qtyResult;
		}

		protected virtual SOLine[] SelectReturnSOLines(string arDocType, string arRefNbr, int? arTranLineNbr)
		{
			if (string.IsNullOrEmpty(arDocType) || string.IsNullOrEmpty(arRefNbr) || arTranLineNbr == null)
				return Array<SOLine>.Empty;

			var query = new SelectFrom<SOLine>
				.InnerJoin<SOOrder>
					.On<SOLine.FK.Order>
				.Where<
					SOLine.invoiceType.IsEqual<@P.AsString.ASCII>
					.And<SOLine.invoiceNbr.IsEqual<@P.AsString>>
					.And<SOLine.invoiceLineNbr.IsEqual<@P.AsInt>>
					.And<SOLine.operation.IsEqual<SOOperation.receipt>>
					.And<SOOrder.cancelled.IsEqual<False>>>
				.View.ReadOnly(Base);

			SOLine[] lines;
			using (new PXFieldScope(query.View, typeof(SOLine)))
				lines = query.SelectMain(arDocType, arRefNbr, arTranLineNbr);

			var linesCache = Base.Caches<SOLine>();
			if (linesCache.IsInsertedUpdatedDeleted)
			{
				var GetOrder = Func.Memorize((string orderType, string orderNbr) =>
					PXParentAttribute.SelectParent<SOOrder>(linesCache, new SOLine { OrderType = orderType, OrderNbr = orderNbr })
					?? (SOOrder)Base.Caches<SOOrder>().Current);

				bool IsReturnLine(SOLine line) =>
					line.InvoiceType == arDocType
					&& line.InvoiceNbr == arRefNbr
					&& line.InvoiceLineNbr == arTranLineNbr
					&& line.Operation == SOOperation.Receipt
					&& GetOrder(line.OrderType, line.OrderNbr)?.Cancelled == false;

				var linesSet = new HashSet<SOLine>(new KeyValuesComparer<SOLine>(linesCache, linesCache.BqlKeys));

				linesSet.AddRange(linesCache.Inserted
					.Concat_(linesCache.Updated)
					.RowCast<SOLine>()
					.Where(IsReturnLine)
					.ToArray());

				linesSet.UnionWith(lines);

				linesSet.ExceptWith(linesCache.Deleted
					.RowCast<SOLine>()
					.Where(IsReturnLine));

				lines = linesSet.ToArray();
			}
			return lines;
		}

		protected virtual IEnumerable<ARTran> SelectReturnARTrans(string arDocType, string arRefNbr)
		{
			if (string.IsNullOrEmpty(arDocType) || string.IsNullOrEmpty(arRefNbr))
				return Array<ARTran>.Empty;

			PXSelectBase<ARTran> selectReturnARTrans = new SelectFrom<ARTran>.
				Where<
					ARTran.sOOrderNbr.IsNull.
					And<ARTran.origInvoiceType.IsEqual<@P.AsString.ASCII>>.
					And<ARTran.origInvoiceNbr.IsEqual<@P.AsString>>.
					And<ARTran.qty.Multiply<ARTran.invtMult>.IsGreater<decimal0>>>.
				View(Base);

			return selectReturnARTrans.Select(arDocType, arRefNbr).RowCast<ARTran>();
		}

		public virtual IEnumerable<InvoiceSplit> SelectInvoicedRecords(string arDocType, string arRefNbr, int? arLineNbr)
		{
			return SelectInvoicedRecords(arDocType, arRefNbr, arLineNbr, includeDirectLines: false);
		}

		protected virtual IEnumerable<InvoiceSplit> SelectInvoicedRecords(string arDocType, string arRefNbr, int? arLineNbr, bool includeDirectLines)
		{
			var cmd = new SelectFrom<InvoiceSplitExpanded>.
				Where<InvoiceSplitExpanded.aRDocType.IsEqual<@P.AsString.ASCII>
					.And<InvoiceSplitExpanded.aRRefNbr.IsEqual<@P.AsString>>
					.And<InvoiceSplitExpanded.aRLineNbr.IsEqual<@P.AsInt>>>
				.View.ReadOnly(Base);

			if (!includeDirectLines)
				cmd.WhereAnd<Where<InvoiceSplitExpanded.aRlineType.IsEqual<InvoiceSplitExpanded.sOlineType>
					.And<InvoiceSplitExpanded.sOOrderNbr.IsNotNull>>>();

			return cmd.Select(arDocType, arRefNbr, arLineNbr).RowCast<InvoiceSplitExpanded>().ToArray();
		}

		protected virtual ReturnedQtyResult CheckInvoicedAndReturnedQty(
			int? returnInventoryID,
			IEnumerable<InvoiceSplit> invoiced,
			IEnumerable<ReturnRecord> returned)
		{
			if (returnInventoryID == null)
				return new ReturnedQtyResult(true);

			int origInventoryID = 0;
			decimal totalInvoicedQty = 0;
			var totalInvoicedQtyByComponent = new Dictionary<int, decimal>();
			var componentsInAKit = new Dictionary<int, decimal>();

			//invoiced are always either KIT or a regular item
			var invoicedGrouppedByARTran = invoiced.GroupBy(r => (r.ARDocType, r.ARRefNbr, r.ARLineNbr, r.InventoryID, r.ARTranQty, r.ARTranUOM, r.ARTranDrCr));
			foreach (var record in invoicedGrouppedByARTran)
			{
				origInventoryID = record.Key.InventoryID.Value;
				decimal invoicedQty = (record.Key.ARTranDrCr == DrCr.Debit ? -1m : 1m) * (decimal)record.Key.ARTranQty;
				totalInvoicedQty += INUnitAttribute.ConvertToBase(Base.Caches<ARTran>(), origInventoryID, record.Key.ARTranUOM, invoicedQty, INPrecision.QUANTITY);

				var details = record.Where(r => r.INTranQty != null)
					.GroupBy(r => (r.INDocType, r.INRefNbr, r.INLineNbr, r.ComponentID, r.INTranUOM, r.INTranQty));

				foreach (var detail in details)
				{
					var inventoryID = detail.Key.ComponentID ?? record.Key.InventoryID.Value;
					if (!totalInvoicedQtyByComponent.ContainsKey(inventoryID))
						totalInvoicedQtyByComponent[inventoryID] = 0;

					totalInvoicedQtyByComponent[inventoryID] +=
						INUnitAttribute.ConvertToBase(Base.Caches<INTran>(), inventoryID, detail.Key.INTranUOM, detail.Key.INTranQty.Value, INPrecision.QUANTITY);
				}
			}

			decimal invoiceQtySign = totalInvoicedQty > 0 ? 1m : -1m;

			foreach (KeyValuePair<int, decimal> kv in totalInvoicedQtyByComponent)
				componentsInAKit[kv.Key] = kv.Value / totalInvoicedQty;

			//returned can be a regular item or a kit or a component of a kit. 
			foreach (var ret in returned)
			{
				if (ret.InventoryID == origInventoryID || totalInvoicedQtyByComponent.Count == 0)//regular item or a kit
				{
					decimal returnedQty = INUnitAttribute.ConvertToBase(LineCache, ret.InventoryID, ret.UOM, ret.Qty, INPrecision.QUANTITY);
					totalInvoicedQty -= returnedQty;

					InventoryItem item = InventoryItem.PK.Find(Base, ret.InventoryID);
					if (item.KitItem == true)
					{
						foreach (KeyValuePair<int, decimal> kv in componentsInAKit)
						{
							totalInvoicedQtyByComponent[kv.Key] -= componentsInAKit[kv.Key] * returnedQty;
						}
					}
				}
				else //component of a kit. 
				{
					totalInvoicedQtyByComponent[ret.InventoryID.Value] -= INUnitAttribute.ConvertToBase(LineCache, ret.InventoryID, ret.UOM, ret.Qty, INPrecision.QUANTITY);
				}
			}

			bool success = true;
			decimal qtyAvailForReturn = 0m;
			if (returnInventoryID == origInventoryID)
			{
				if (invoiceQtySign * totalInvoicedQty < 0m || totalInvoicedQtyByComponent.Values.Any(v => invoiceQtySign * v < 0m))
					success = false;

				qtyAvailForReturn = invoiceQtySign * totalInvoicedQty;

				foreach (var invoiceQtyByComponent in totalInvoicedQtyByComponent)
				{
					int componentID = invoiceQtyByComponent.Key;
					decimal invoicedQty = invoiceQtyByComponent.Value * invoiceQtySign;
					if (componentsInAKit.TryGetValue(componentID, out decimal componentMult))
					{
						qtyAvailForReturn = Math.Min(qtyAvailForReturn, invoicedQty / componentMult);
					}
				}
			}
			else
			{
				if (invoiceQtySign * totalInvoicedQty < 0m)
					success = false;

				qtyAvailForReturn = invoiceQtySign * totalInvoicedQty;

				if (totalInvoicedQtyByComponent.TryGetValue(returnInventoryID.Value, out decimal qtyByComponent))
				{
					if (invoiceQtySign * qtyByComponent < 0)
						success = false;

					qtyAvailForReturn = invoiceQtySign * qtyByComponent;
				}
			}

			InventoryItem returnedItem = InventoryItem.PK.Find(Base, returnInventoryID);
			if (returnedItem.DecimalBaseUnit != true || returnedItem.DecimalSalesUnit != true)
			{
				qtyAvailForReturn = decimal.Truncate(qtyAvailForReturn);
			}
			else
			{
				qtyAvailForReturn = Math.Round(qtyAvailForReturn, CommonSetupDecPl.Qty, MidpointRounding.AwayFromZero);
			}

			return new ReturnedQtyResult(success, success ? null : returned.ToArray(), qtyAvailForReturn);
		}

		[PXInternalUseOnly]
		public abstract class ReturnRecord
		{
			public abstract int? InventoryID { get; }
			public abstract string UOM { get; }
			public abstract decimal Qty { get; }
			public abstract string DocumentNbr { get; }

			public static ReturnRecord FromSOLine(SOLine l) => new ReturnSOLine(l);

			public static ReturnRecord FromARTran(ARTran t) => new ReturnARTran(t);

			private class ReturnSOLine : ReturnRecord
			{
				public ReturnSOLine(SOLine line) => Line = line;

				public SOLine Line { get; }
				public override string DocumentNbr => Line.OrderNbr;
				public override int? InventoryID => Line.InventoryID;
				public override string UOM => Line.UOM;
				public override decimal Qty => (Line.InvtMult *
					(Line.LineType != SOLineType.MiscCharge && Line.RequireShipping == true && Line.Completed == true
						? Line.ShippedQty : Line.OrderQty)) ?? 0m;
			}

			private class ReturnARTran : ReturnRecord
			{
				public ReturnARTran(ARTran tran) => Tran = tran;

				public ARTran Tran { get; }
				public override string DocumentNbr => Tran.RefNbr;
				public override int? InventoryID => Tran.InventoryID;
				public override string UOM => Tran.UOM;
				public override decimal Qty => Math.Abs(Tran.Qty ?? 0m);
			}
		}

		public class ReturnedQtyResult
		{
			public ReturnedQtyResult(bool success, ReturnRecord[] returnRecords = null, decimal? qtyAvailForReturn = null)
			{
				Success = success;
				ReturnRecords = returnRecords;
				QtyAvailForReturn = qtyAvailForReturn;
			}

			public bool Success { get; private set; }
			public ReturnRecord[] ReturnRecords { get; private set; }
			public decimal? QtyAvailForReturn { get; private set; }
		}
	}
}
