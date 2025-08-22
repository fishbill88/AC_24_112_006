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
using PX.Data;
using PX.Objects.Common.Exceptions;
using PX.Objects.CS;
using PX.Objects.Extensions;
using PX.Objects.SO;
using PX.Objects.SO.GraphExtensions.SOOrderEntryExt;
using SOLine5 = PX.Objects.PO.POOrderEntry.SOLine5;

namespace PX.Objects.PO.GraphExtensions.POOrderEntryExt
{
	public class AffectedSpecialOrderLinesByPOLines : ProcessAffectedEntitiesInPrimaryGraphBase<AffectedSpecialOrderLinesByPOLines, POOrderEntry, SOOrder, SOOrderEntry>
	{
		public static bool IsActive() => PXAccess.FeatureInstalled<FeaturesSet.specialOrders>();

		private Dictionary<SOOrder, IEnumerable<SOLine5>> _affectedOrders;

		protected override bool PersistInSameTransaction => true;

		protected override bool EntityIsAffected(SOOrder entity) => false;

		protected override IEnumerable<SOOrder> GetAffectedEntities()
		{
			foreach (var g in Base.FixedDemandOrigSOLine.Cache.Updated.Cast<SOLine5>()
				.Where(l => l.IsCostUpdatedOnPO == true && l.CuryUnitCostUpdated != null)
				.GroupBy(l => new { l.OrderType, l.OrderNbr }))
			{
				_affectedOrders ??= new Dictionary<SOOrder, IEnumerable<SOLine5>>(Base.Caches<SOOrder>().GetComparer());
				_affectedOrders.Add(
					SOOrder.PK.Find(Base, g.First().OrderType, g.First().OrderNbr),
					g);
			}
			return _affectedOrders?.Keys ?? Enumerable.Empty<SOOrder>();
		}

		protected override SOOrder ActualizeEntity(SOOrderEntry primaryGraph, SOOrder entity)
			=> primaryGraph.Document.Search<SOOrder.orderNbr>(entity.OrderNbr, entity.OrderType);

		protected override void ProcessAffectedEntity(SOOrderEntry primaryGraph, SOOrder entity)
		{
			foreach (var affectedLine in _affectedOrders[entity])
			{
				SOLine soLine = primaryGraph.Transactions.Search<SOLine.lineNbr>(affectedLine.LineNbr, affectedLine.OrderType, affectedLine.OrderNbr)
					?? throw new RowNotFoundException(primaryGraph.Transactions.Cache, affectedLine.OrderType, affectedLine.OrderNbr, affectedLine.LineNbr);
				soLine.CuryUnitCost = affectedLine.CuryUnitCostUpdated;
				using (primaryGraph.FindImplementation<SOLineCost>()?.CostsRecalculationScope())
				{
					soLine = primaryGraph.Transactions.Update(soLine);
				}
			}
		}

		protected override void OnProcessed(SOOrderEntry foreignGraph)
		{
			base.OnProcessed(foreignGraph);
			_affectedOrders = null;
		}

		protected override void ClearCaches(PXGraph graph)
		{
			base.ClearCaches(graph);
			graph.Caches<SOLine5>().Clear();
			graph.Caches<SOLine5>().ClearQueryCache();
		}
	}
}
