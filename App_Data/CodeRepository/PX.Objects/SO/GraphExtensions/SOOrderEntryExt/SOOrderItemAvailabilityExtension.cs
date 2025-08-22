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
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.Common.Exceptions;
using PX.Objects.CS;
using PX.Objects.IN;

namespace PX.Objects.SO.GraphExtensions.SOOrderEntryExt
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class SOOrderItemAvailabilityExtension : SOBaseItemAvailabilityExtension<SOOrderEntry, SOLine, SOLineSplit>
	{
		protected override SOLineSplit EnsureSplit(ILSMaster row)
			=> Base.FindImplementation<SOOrderLineSplittingExtension>().EnsureSplit(row);

		protected override decimal GetUnitRate(SOLine line) => GetUnitRate<SOLine.inventoryID, SOLine.uOM>(line);


		protected override string GetStatus(SOLine line)
		{
			string status = string.Empty;

			bool excludeCurrent = line?.Completed != true && line?.InvtMult != 0;

			if (FetchWithLineUOM(line, excludeCurrent, line.CostCenterID) is IStatus availability)
			{
				status = FormatStatus(availability, line.UOM);
				Check(line, availability);
			}

			return status;
		}

		private string FormatStatus(IStatus availability, string uom)
		{
			return PXMessages.LocalizeFormatNoPrefixNLA(
				Messages.Availability_Info,
				uom,
				FormatQty(availability.QtyOnHand),
				FormatQty(availability.QtyAvail),
				FormatQty(availability.QtyHardAvail));
		}


		public override void Check(ILSMaster row, int? costCenterID)
		{
			base.Check(row, costCenterID);
			MemoCheck(row);
		}

		protected virtual void MemoCheck(ILSMaster row)
		{
			if (row is SOLine line)
			{
				MemoCheck(line);

				SOLineSplit split = EnsureSplit(line);
				MemoCheck(line, split, triggeredBySplit: false);

				if (split.LotSerialNbr == null)
					row.LotSerialNbr = null;
			}
			else if (row is SOLineSplit split)
			{
				line = PXParentAttribute.SelectParent<SOLine>(SplitCache, split);
				MemoCheck(line);
				MemoCheck(line, split, triggeredBySplit: true);
			}
		}

		protected virtual bool MemoCheck(SOLine line) => MemoCheck(line, false);
		public virtual bool MemoCheck(SOLine line, bool persisting)
		{
			if (line.Operation == SOOperation.Issue)
				return true;

			var result = MemoCheckQty(line);
			if (result.Success != true)
			{
				var documents = result.ReturnRecords?
					.Select(x => x.DocumentNbr)
					.Where(nbr => nbr != line.OrderNbr);

				RaiseErrorOnOrderQty(persisting, line, Messages.InvoiceCheck_DecreaseQty,
					LineCache.GetValueExt<SOLine.invoiceNbr>(line),
					LineCache.GetValueExt<SOLine.inventoryID>(line),
					documents == null ? string.Empty : string.Join(", ", documents));
			}

			return result.Success;
		}

		protected virtual ReturnedQtyResult MemoCheckQty(SOLine line)
		{
			return MemoCheckQty(line.InventoryID, line.InvoiceType, line.InvoiceNbr, line.InvoiceLineNbr, line.OrigOrderType, line.OrigOrderNbr, line.OrigLineNbr);
		}

		public virtual (bool success, decimal qtyAvailForReturn) MemoCheck(SOLine line, SOLineSplit split, bool triggeredBySplit, bool raiseException = true)
		{
			bool success = true;
			decimal qtyAvailForReturn = 0m;
			if (line.InvoiceNbr != null && split.InventoryID != null)
			{
				var item = InventoryItem.PK.Find(Base, split.InventoryID);
				var lsClass = item?.StkItem == true ? INLotSerClass.PK.Find(Base, item.LotSerClassID) : new INLotSerClass();

				if (lsClass.LotSerTrack.IsIn(INLotSerTrack.LotNumbered, INLotSerTrack.SerialNumbered) && split.SubItemID != null && string.IsNullOrEmpty(split.LotSerialNbr) == false)
				{
					var origLine = (PXResult<INTran, INTranSplit>)
						SelectFrom<INTran>.
						InnerJoin<INTranSplit>.On<INTranSplit.FK.Tran>.
						Where<
							INTran.sOOrderType.IsEqual<SOLine.origOrderType.AsOptional>.
							And<INTran.sOOrderNbr.IsEqual<SOLine.origOrderNbr.AsOptional>>.
							And<INTran.sOOrderLineNbr.IsEqual<SOLine.origLineNbr.AsOptional>>.
							And<INTran.aRDocType.IsEqual<SOLine.invoiceType.AsOptional>>.
							And<INTran.aRRefNbr.IsEqual<SOLine.invoiceNbr.AsOptional>>.
							And<INTranSplit.inventoryID.IsEqual<SOLineSplit.inventoryID.AsOptional>>.
							And<INTranSplit.subItemID.IsEqual<SOLineSplit.subItemID.AsOptional>>.
							And<INTranSplit.lotSerialNbr.IsEqual<SOLineSplit.lotSerialNbr.AsOptional>>>.
						AggregateTo<
							GroupBy<INTranSplit.inventoryID>,
							GroupBy<INTranSplit.subItemID>,
							GroupBy<INTranSplit.lotSerialNbr>,
							Sum<INTranSplit.baseQty>>.
						View.SelectSingleBound(Base, new object[] { line, split });

					if (origLine == null)
					{
						if (string.IsNullOrEmpty(split.AssignedNbr) == false && INLotSerialNbrAttribute.StringsEqual(split.AssignedNbr, split.LotSerialNbr))
						{
							split.AssignedNbr = null;
							split.LotSerialNbr = null;
						}
						else
						{
							if (raiseException)
								RaiseMemoQtyExceptionHanding(line, split, triggeredBySplit, new PXSetPropertyException(Messages.InvoiceCheck_LotSerialInvalid));

							success = false;
						}
						return (success, 0m);
					}


					decimal? qtyInvoicedLotBase = origLine.GetItem<INTranSplit>().BaseQty;


					var memoLine = (PXResult<SOLine, SOLineSplit>)
						SelectFrom<SOLine>.
						InnerJoin<SOLineSplit>.On<SOLineSplit.FK.OrderLine>.
						Where<
							Brackets<
								SOLine.orderType.IsNotEqual<SOLine.orderType.AsOptional>.
								Or<SOLine.orderNbr.IsNotEqual<SOLine.orderNbr.AsOptional>>>.
							And<SOLine.origOrderType.IsEqual<SOLine.origOrderType.AsOptional>>.
							And<SOLine.origOrderNbr.IsEqual<SOLine.origOrderNbr.AsOptional>>.
							And<SOLine.origLineNbr.IsEqual<SOLine.origLineNbr.AsOptional>>.
							And<SOLine.invoiceType.IsEqual<SOLine.invoiceType.AsOptional>>.
							And<SOLine.invoiceNbr.IsEqual<SOLine.invoiceNbr.AsOptional>>.
							And<SOLine.operation.IsEqual<SOOperation.receipt>>.
							And<SOLineSplit.inventoryID.IsEqual<SOLineSplit.inventoryID.AsOptional>>.
							And<SOLineSplit.subItemID.IsEqual<SOLineSplit.subItemID.AsOptional>>.
							And<SOLineSplit.lotSerialNbr.IsEqual<SOLineSplit.lotSerialNbr.AsOptional>>.
							And<
								SOLine.baseBilledQty.IsGreater<decimal0>.
								Or<
									SOLine.baseShippedQty.IsGreater<decimal0>.
									And<SOLineSplit.baseShippedQty.IsGreater<decimal0>>>.
								Or<
									SOLine.completed.IsNotEqual<True>.
									And<SOLineSplit.completed.IsNotEqual<True>>>>>.
						AggregateTo<
							GroupBy<SOLineSplit.inventoryID>,
							GroupBy<SOLineSplit.subItemID>,
							GroupBy<SOLineSplit.lotSerialNbr>,
							Sum<SOLineSplit.baseQty>,
							Sum<SOLineSplit.baseShippedQty>>.
						View.SelectSingleBound(Base, new object[] { line, split });

					if (memoLine != null)
					{
						if (lsClass.LotSerTrack == INLotSerTrack.SerialNumbered)
						{
							if (raiseException)
								RaiseMemoQtyExceptionHanding(line, split, triggeredBySplit, new PXSetPropertyException(Messages.InvoiceCheck_SerialAlreadyReturned));

							success = false;
						}
						else
						{
							decimal returnedQty = memoLine.GetItem<SOLineSplit>().BaseShippedQty ?? 0m;
							if (returnedQty == 0)
								returnedQty = memoLine.GetItem<SOLineSplit>().BaseQty ?? 0m;

							qtyInvoicedLotBase -= returnedQty;
						}
					}

					qtyInvoicedLotBase -= PXParentAttribute
						.SelectSiblings(SplitCache, split, typeof(SOLine))
						.Cast<SOLineSplit>()
						.Where(s =>
							s.SubItemID == split.SubItemID &&
							string.Equals(s.LotSerialNbr, split.LotSerialNbr, StringComparison.OrdinalIgnoreCase))
						.Sum(s => s.Completed == true ? s.BaseShippedQty : s.BaseQty);

					qtyAvailForReturn = qtyInvoicedLotBase ?? 0m;

					if (qtyInvoicedLotBase < 0m)
					{
						if (raiseException)
							RaiseMemoQtyExceptionHanding(line, split, triggeredBySplit, new PXSetPropertyException(Messages.InvoiceCheck_QtyLotSerialNegative));

						success = false;
					}
				}
			}

			return (success, success ? qtyAvailForReturn : 0m);
		}

		protected void RaiseMemoQtyExceptionHanding(SOLine line, SOLineSplit split, bool triggeredBySplit, Exception e)
		{
			if (triggeredBySplit)
			{
				SplitCache.RaiseExceptionHandling<SOLineSplit.qty>(split, split.Qty,
					new PXSetPropertyException(e.Message,
						LineCache.GetValueExt<SOLine.inventoryID>(line),
						SplitCache.GetValueExt<SOLineSplit.subItemID>(split),
						LineCache.GetValueExt<SOLine.invoiceNbr>(line),
						SplitCache.GetValueExt<SOLineSplit.lotSerialNbr>(split)));
			}
			else
			{
				LineCache.RaiseExceptionHandling<SOLine.orderQty>(line, line.OrderQty,
					new PXSetPropertyException(e.Message,
						LineCache.GetValueExt<SOLine.inventoryID>(line),
						LineCache.GetValueExt<SOLine.subItemID>(line),
						LineCache.GetValueExt<SOLine.invoiceNbr>(line),
						LineCache.GetValueExt<SOLine.lotSerialNbr>(line)));
			}
		}


		protected override void Optimize()
		{
			base.Optimize();

			foreach (PXResult<SOLineShort, INUnit, INSiteStatusByCostCenter> res in
				SelectFrom<SOLineShort>.
				InnerJoin<INUnit>.On<
					INUnit.inventoryID.IsEqual<SOLineShort.inventoryID>.
					And<INUnit.fromUnit.IsEqual<SOLineShort.uOM>>>.
				InnerJoin<INSiteStatusByCostCenter>.On<
					SOLineShort.FK.SiteStatusByCostCenter>.
				Where<SOLineShort.FK.Order.SameAsCurrent>.
				View.ReadOnly.Select(Base))
			{
				INUnit.UK.ByInventory.StoreResult(Base, res);
				INSiteStatusByCostCenter.PK.StoreResult(Base, res);
			}
		}

		protected override void RaiseQtyExceptionHandling(SOLine line, PXExceptionInfo ei, decimal? newValue)
		{
			LineCache.RaiseExceptionHandling<SOLine.orderQty>(line, newValue,
				new PXSetPropertyException(ei.MessageFormat, PXErrorLevel.Warning,
					LineCache.GetStateExt<SOLine.inventoryID>(line),
					LineCache.GetStateExt<SOLine.subItemID>(line),
					LineCache.GetStateExt<SOLine.siteID>(line),
					LineCache.GetStateExt<SOLine.locationID>(line),
					LineCache.GetValue<SOLine.lotSerialNbr>(line)));
		}

		protected override void RaiseQtyExceptionHandling(SOLineSplit split, PXExceptionInfo ei, decimal? newValue)
		{
			SplitCache.RaiseExceptionHandling<SOLineSplit.qty>(split, newValue,
				new PXSetPropertyException(ei.MessageFormat, PXErrorLevel.Warning,
					SplitCache.GetStateExt<SOLineSplit.inventoryID>(split),
					SplitCache.GetStateExt<SOLineSplit.subItemID>(split),
					SplitCache.GetStateExt<SOLineSplit.siteID>(split),
					SplitCache.GetStateExt<SOLineSplit.locationID>(split),
					SplitCache.GetValue<SOLineSplit.lotSerialNbr>(split)));
		}

		protected virtual void RaiseErrorOnOrderQty(bool onPersist, SOLine line, string errorMessage, params object[] args)
		{
			var propertyException = new PXSetPropertyKeepPreviousException(errorMessage, PXErrorLevel.Error, args);
			decimal? value = line.LineSign * line.InvtMult * line.OrderQty;
			if (LineCache.RaiseExceptionHandling<SOLine.orderQty>(line, value, propertyException) && onPersist)
				throw new PXRowPersistingException(typeof(SOLine.orderQty).Name, value, errorMessage, args);
		}
	}
}
