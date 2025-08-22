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

using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.Extensions;
using PX.Objects.IN;
using System.Collections.Generic;
using PX.Objects.Common.Extensions;
using PX.Objects.CS;

namespace PX.Objects.PO.GraphExtensions
{
	public abstract class AffectedPOOrdersByPOLine<TSelf, TGraph> : ProcessAffectedEntitiesInPrimaryGraphBase<TSelf, TGraph, POOrder, POOrderEntry>
		where TGraph : PXGraph
		where TSelf : AffectedPOOrdersByPOLine<TSelf, TGraph>
	{
		#region Overrides

		protected override bool PersistInSameTransaction => true;

		protected override bool EntityIsAffected(POOrder entity)
		{
			var cache = Base.Caches<POOrder>();
			int? linesToCloseCntrOldValue = (int?)cache.GetValueOriginal<POOrder.linesToCloseCntr>(entity),
				linesToCompleteCntrOldValue = (int?)cache.GetValueOriginal<POOrder.linesToCompleteCntr>(entity);
			return
				(!Equals(linesToCloseCntrOldValue, entity.LinesToCloseCntr)
				|| !Equals(linesToCompleteCntrOldValue, entity.LinesToCompleteCntr))
				&& (linesToCloseCntrOldValue == 0 || linesToCompleteCntrOldValue == 0
				|| entity.LinesToCloseCntr == 0 || entity.LinesToCompleteCntr == 0);
		}

		protected override IEnumerable<POOrder> GetAffectedEntities()
			=> base.GetAffectedEntities().BeginWith(x => x.OrderType != POOrderType.Blanket);

		protected override void ProcessAffectedEntity(POOrderEntry primaryGraph, POOrder entity)
		{
			primaryGraph.UpdateDocumentState(entity);
		}

		protected override POOrder ActualizeEntity(POOrderEntry primaryGraph, POOrder entity)
			=> PXSelect<POOrder,
				Where<POOrder.orderType, Equal<Required<POOrder.orderType>>,
					And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>>
				.Select(primaryGraph, entity.OrderType, entity.OrderNbr);

		#endregion

		#region POLine fields overriding

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXParent(typeof(SelectFrom<POLine>
			.Where<POLine.orderType.IsEqual<POOrderType.blanket>
				.And<POLine.orderType.IsEqual<POLine.pOType.FromCurrent>>
				.And<POLine.orderNbr.IsEqual<POLine.pONbr.FromCurrent>>
				.And<POLine.lineNbr.IsEqual<POLine.pOLineNbr.FromCurrent>>>))]
		protected virtual void _(Events.CacheAttached<POLine.pOLineNbr> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXUnboundFormula(
			typeof(Switch<
				Case<Where<POLine.cancelled.IsEqual<True>>, decimal0,
				Case<Where<POLine.completed.IsEqual<True>.And<POLine.lineType.IsNotEqual<POLineType.service>>>, POLine.baseReceivedQty>>,
				POLine.baseOrderQty>),
			typeof(SumCalc<POLine.baseOrderedQty>))]
		protected virtual void _(Events.CacheAttached<POLine.completed> e) { }

		#endregion

		#region Update blanket order line

		protected virtual void _(Events.RowUpdated<POLine> e)
		{
			if (e.Row?.POType != POOrderType.Blanket)
				return;

			var blanketRow = UpdateBlanketRow(e.Cache, e.Row, e.OldRow, false);
			if (blanketRow != null)
				e.Cache.Current = e.Row;
		}

		protected virtual POLine UpdateBlanketRow(PXCache cache, POLine normalRow, POLine normalOldRow, bool hardCheck)
		{
			if (!hardCheck && cache.ObjectsEqual<POLine.completed, POLine.closed>(normalRow, normalOldRow))
				return null;

			var blanketRow = FindBlanketRow(cache, normalRow);
			if (blanketRow == null)
				throw new PXArgumentException(nameof(blanketRow));

			bool? oldCompleted = blanketRow.Completed;
			bool? oldClosed = blanketRow.Closed;

			if (hardCheck
				|| normalRow.Completed != normalOldRow.Completed)
			{
				blanketRow = CompleteBlanketRow(cache, blanketRow, normalRow, normalOldRow);
			}

			if (hardCheck
				|| oldCompleted != blanketRow.Completed
				|| normalRow.Closed != normalOldRow.Closed)
			{
				blanketRow = CloseBlanketRow(cache, blanketRow, normalRow, normalOldRow);
			}

			if (oldCompleted != blanketRow.Completed || oldClosed != blanketRow.Closed)
			{
				return (POLine)cache.Update(blanketRow);
			}

			return null;
		}

		protected POLine FindBlanketRow(PXCache rowCache, POLine normalRow)
			=> PXParentAttribute.SelectParent<POLine>(rowCache, normalRow);

		protected virtual POLine CompleteBlanketRow(PXCache cache, POLine blanketRow, POLine normalRow, POLine normalOldRow)
		{
			bool completed;
			if (normalRow.Completed == false)
			{
				completed = false;
			}
			else
			{
				if (blanketRow.CompletePOLine == CompletePOLineTypes.Quantity)
				{
					if(POLineType.IsService(blanketRow.LineType))
						completed = blanketRow.BaseBilledQty >= blanketRow.BaseOrderQty * blanketRow.RcptQtyThreshold / 100;
					else
						completed = blanketRow.BaseReceivedQty >= blanketRow.BaseOrderQty * blanketRow.RcptQtyThreshold / 100;
				}
				else
				{
					completed = blanketRow.BilledAmt >= (blanketRow.ExtCost + blanketRow.RetainageAmt) * blanketRow.RcptQtyThreshold / 100;
				}
				if (completed)
				{
					POLine uncompletedChild = SelectFrom<POLine>
						.Where<POLine.FK.BlanketLine.SameAsCurrent
							.And<POLine.completed.IsEqual<False>>
							.And<POLine.orderType.IsNotEqual<@P.AsString.ASCII>
								.Or<POLine.orderNbr.IsNotEqual<@P.AsString>>
								.Or<POLine.lineNbr.IsNotEqual<@P.AsInt>>>
							>
						.View
						.SelectSingleBound(Base, new[] { blanketRow }, normalRow.OrderType, normalRow.OrderNbr, normalRow.LineNbr);
					completed = uncompletedChild == null;
				}
			}

			blanketRow.Completed = completed;

			return blanketRow;
		}

		protected virtual POLine CloseBlanketRow(PXCache cache, POLine blanketRow, POLine normalRow, POLine normalOldRow)
		{
			bool closed;
			if (blanketRow.Completed == false)
			{
				closed = false;
			}
			else if(normalRow.Closed == false)
			{
				closed = false;
			}
			else
			{
				POLine unclosedChild = SelectFrom<POLine>
					.Where<POLine.FK.BlanketLine.SameAsCurrent
						.And<POLine.closed.IsEqual<False>>
						.And<POLine.orderType.IsNotEqual<@P.AsString.ASCII>
							.Or<POLine.orderNbr.IsNotEqual<@P.AsString>>
							.Or<POLine.lineNbr.IsNotEqual<@P.AsInt>>>
						>
					.View
					.SelectSingleBound(Base, new[] { blanketRow }, normalRow.OrderType, normalRow.OrderNbr, normalRow.LineNbr);
				closed = unclosedChild == null;
			}

			blanketRow.Closed = closed;

			return blanketRow;
		}

		#endregion
	}
}
