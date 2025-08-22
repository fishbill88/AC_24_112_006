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
using PX.Objects.Extensions.MultiCurrency;
using PX.Objects.IN;
using PX.Objects.PM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.PO.GraphExtensions
{
	public abstract class BaseUpdatePOAccrual<TMultiCurrencyExtension, TGraph, TPrimaryDAC> : PXGraphExtension<TMultiCurrencyExtension, TGraph>
		where TMultiCurrencyExtension: MultiCurrencyGraph<TGraph, TPrimaryDAC>
		where TGraph : PXGraph
		where TPrimaryDAC : class, IBqlTable, new()
	{
		#region Cache Attached

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[POUnbilledTaxR(typeof(POOrder), typeof(POTax), typeof(POTaxTran),
			//Per Unit Tax settings
			Inventory = typeof(POLine.inventoryID), UOM = typeof(POLine.uOM), LineQty = typeof(POLine.unbilledQty))]
		protected virtual void _(Events.CacheAttached<POLine.taxCategoryID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(PXDBQuantityAttribute))]
		[PXDBQuantity]
		protected virtual void _(Events.CacheAttached<POLine.orderedQty> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(PXDBDecimalAttribute))]
		[SO.PXDBBaseQtyWithOrigQty(typeof(POLine.uOM), typeof(POLine.orderedQty), typeof(POLine.uOM), typeof(POLine.baseOrderQty), typeof(POLine.orderQty), HandleEmptyKey = true, MinValue = 0)]
		protected virtual void _(Events.CacheAttached<POLine.baseOrderedQty> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[POCommitment]
		protected virtual void _(Events.CacheAttached<POLine.commitmentID> e) { }

		#endregion

		public virtual POAccrualRecord GetAccrualStatusSummary(POLine poLine)
		{
			string orderType = poLine.OrderType;
			string orderNbr = poLine.OrderNbr;
			int? orderLineNbr = poLine.LineNbr;
			string poAccrualType = poLine.POAccrualType;

			if (!poAccrualType.IsIn(POAccrualType.Order, POAccrualType.Receipt))
				return new POAccrualRecord();

			// works for both order-based and receipt-based billing
			var accrualRecords = PXSelect<POAccrualStatus,
				Where<POAccrualStatus.orderType, Equal<Required<POAccrualStatus.orderType>>,
					And<POAccrualStatus.orderNbr, Equal<Required<POAccrualStatus.orderNbr>>,
					And<POAccrualStatus.orderLineNbr, Equal<Required<POAccrualStatus.orderLineNbr>>>>>>
				.Select(Base, orderType, orderNbr, orderLineNbr)
				.RowCast<POAccrualStatus>()
				.Select(POAccrualRecord.FromPOAccrualStatus)
				.ToList();

			return Accumulate(accrualRecords);
		}

		protected virtual POAccrualRecord Accumulate(IEnumerable<POAccrualRecord> records)
		{
			var result = new POAccrualRecord();
			foreach (var r in records)
			{
				bool nulloutReceivedQty = result.ReceivedQty == null || r.ReceivedQty == null
					|| result.ReceivedUOM != null && r.ReceivedUOM != null && !string.Equals(result.ReceivedUOM, r.ReceivedUOM, StringComparison.OrdinalIgnoreCase);
				if (nulloutReceivedQty)
				{
					result.ReceivedUOM = null;
					result.ReceivedQty = null;
				}
				else if (r.ReceivedQty != 0m)
				{
					result.ReceivedUOM = r.ReceivedUOM;
					result.ReceivedQty += r.ReceivedQty;
				}
				result.BaseReceivedQty += r.BaseReceivedQty;
				result.ReceivedCost += r.ReceivedCost;
				bool nulloutBilledQty = result.BilledQty == null || r.BilledQty == null
					|| result.BilledUOM != null && r.BilledUOM != null && !string.Equals(result.BilledUOM, r.BilledUOM, StringComparison.OrdinalIgnoreCase);
				if (nulloutBilledQty)
				{
					result.BilledUOM = null;
					result.BilledQty = null;
				}
				else if (r.BilledQty != 0m)
				{
					result.BilledUOM = r.BilledUOM;
					result.BilledQty += r.BilledQty;
				}
				result.BaseBilledQty += r.BaseBilledQty;
				bool nulloutBilledCuryAmt = result.CuryBilledAmt == null || r.CuryBilledAmt == null
					|| result.BillCuryID != null && r.BillCuryID != null && !string.Equals(result.BillCuryID, r.BillCuryID, StringComparison.OrdinalIgnoreCase);
				if (nulloutBilledCuryAmt)
				{
					result.BillCuryID = null;
					result.CuryBilledAmt = null;
					result.CuryBilledCost = null;
					result.CuryBilledDiscAmt = null;
				}
				else if (r.CuryBilledAmt != 0m)
				{
					result.BillCuryID = r.BillCuryID;
					result.CuryBilledAmt += r.CuryBilledAmt;
					result.CuryBilledCost += r.CuryBilledCost;
					result.CuryBilledDiscAmt += r.CuryBilledDiscAmt;
				}
				result.BilledAmt += r.BilledAmt;
				result.BilledCost += r.BilledCost;
				result.BilledDiscAmt += r.BilledDiscAmt;
				result.PPVAmt += r.PPVAmt;
			}
			return result;
		}

		protected void SetIfNotNull<TField>(PXCache cache, POAccrualStatus row, object value)
			where TField : IBqlField
		{
			if (value != null)
			{
				cache.SetValue<TField>(row, value);
			}
		}

		protected void SetIfNotEmpty<TField>(PXCache cache, POAccrualStatus row, decimal? value)
			where TField : IBqlField
		{
			if (value != null && value != 0m)
			{
				cache.SetValue<TField>(row, value);
			}
		}
	}

	public class POAccrualRecord
	{
		public string ReceivedUOM { get; set; }
		public decimal? ReceivedQty { get; set; } = 0m;
		public decimal? BaseReceivedQty { get; set; } = 0m;
		public decimal? ReceivedCost { get; set; } = 0m;
		public string BilledUOM { get; set; }
		public decimal? BilledQty { get; set; } = 0m;
		public decimal? BaseBilledQty { get; set; } = 0m;
		public string BillCuryID { get; set; }
		public decimal? CuryBilledAmt { get; set; } = 0m;
		public decimal? BilledAmt { get; set; } = 0m;
		public decimal? CuryBilledCost { get; set; } = 0m;
		public decimal? BilledCost { get; set; } = 0m;
		public decimal? CuryBilledDiscAmt { get; set; } = 0m;
		public decimal? BilledDiscAmt { get; set; } = 0m;
		public decimal? PPVAmt { get; set; } = 0m;

		public static POAccrualRecord FromPOAccrualStatus(POAccrualStatus s)
		{
			return new POAccrualRecord
			{
				ReceivedUOM = s.ReceivedUOM,
				ReceivedQty = s.ReceivedQty,
				BaseReceivedQty = s.BaseReceivedQty,
				ReceivedCost = s.ReceivedCost,
				BilledUOM = s.BilledUOM,
				BilledQty = s.BilledQty,
				BaseBilledQty = s.BaseBilledQty,
				BillCuryID = s.BillCuryID,
				CuryBilledAmt = s.CuryBilledAmt,
				BilledAmt = s.BilledAmt,
				CuryBilledCost = s.CuryBilledCost,
				BilledCost = s.BilledCost,
				CuryBilledDiscAmt = s.CuryBilledDiscAmt,
				BilledDiscAmt = s.BilledDiscAmt,
				PPVAmt = s.PPVAmt,
			};
		}
	}
}
