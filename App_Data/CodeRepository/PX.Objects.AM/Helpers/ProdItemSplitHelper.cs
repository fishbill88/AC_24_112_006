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
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.AM
{
	public class ProdItemSplitHelper
	{
		public static void BuildPreassignNumbers<TGraph, TRow, TSplit>(TGraph graph, TRow row, TSplit tSplit) where TRow : IProdOrder where TSplit : ILSDetail, new() where TGraph : PXGraph
		{
			Dictionary<string, (decimal? qty, DateTime? expireDate)> AssignedSplits = new Dictionary<string, (decimal? qty, DateTime? expireDate)>();
			foreach (AMProdItemSplit split in SelectFrom<AMProdItemSplit>.Where<AMProdItemSplit.orderType.IsEqual<@P.AsString>
			.And<AMProdItemSplit.prodOrdID.IsEqual<@P.AsString>>>.View.Select(graph, row.OrderType, row.ProdOrdID))
			{
				if (split.QtyRemaining.GetValueOrDefault() > 0)
				{
					(decimal? qty, DateTime? expireDate) splitData = (qty: split.QtyRemaining, expireDate: split.ExpireDate);
					AssignedSplits.Add(split.LotSerialNbr, splitData);
				}
			}
			if (AssignedSplits.Count > 0)
			{
				foreach (IMoveItemSplit detail in PXParentAttribute.SelectSiblings(graph.Caches[typeof(TSplit)], tSplit, typeof(TRow)))
				{
					if (AssignedSplits.Count == 0)
					{
						graph.Caches[typeof(TSplit)].Delete(detail);
						continue;
					}

					if (AssignedSplits.ContainsKey(detail.LotSerialNbr) &&
						AssignedSplits.ElementAt(0).Value.expireDate.HasValue && detail.ExpireDate.HasValue && AssignedSplits[detail.LotSerialNbr].qty > detail.Qty)
					{
						continue;
					}

					if (!AssignedSplits.ContainsKey(detail.LotSerialNbr))
						detail.LotSerialNbr = AssignedSplits.ElementAt(0).Key;
					if (detail.Qty >= AssignedSplits.ElementAt(0).Value.qty)
					{
						detail.Qty = AssignedSplits.ElementAt(0).Value.qty;
					}
					if (AssignedSplits.ElementAt(0).Value.expireDate.HasValue)
					{
						detail.ExpireDate = AssignedSplits.ElementAt(0).Value.expireDate;
					}

					AssignedSplits.Remove(detail.LotSerialNbr);
					graph.Caches[typeof(TSplit)].MarkUpdated(detail);
				}
			}
		}

		public static AMProdItemSplit GetProdItemSplit(PXGraph graph, string orderType, string prodOrdID, string lotSerial = null)
		{
			return PXSelect<AMProdItemSplit, Where<AMProdItemSplit.orderType, Equal<Required<AMProdItemSplit.orderType>>,
				And<AMProdItemSplit.prodOrdID, Equal<Required<AMProdItemSplit.prodOrdID>>,
				And<AMProdItemSplit.lotSerialNbr, Equal<Required<AMProdItemSplit.lotSerialNbr>>>>>>.Select(graph, orderType, prodOrdID, lotSerial);
		}

	}
}
