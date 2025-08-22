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
using PX.Objects.AM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.AM
{
	/// <summary>
	/// Bill of Material data lookup cache for use in <see cref="MRPEngine"/> process and the planned scheduling process of <see cref="SchedulePlanBomCopy"/>.
	/// </summary>
	public class MRPBomCache
	{
		protected class CacheItem
		{
			public readonly List<object> Operations = new List<object>();
			public readonly Dictionary<int, List<object>> OperationPhantoms = new Dictionary<int, List<object>>();
		}
		protected readonly PXGraph graph;
		protected readonly Dictionary<AMBomItem, CacheItem> boms;
		protected readonly Dictionary<string, List<AMBomItem>> bomRevisions;
		public readonly List<object> AllMaterialCache = new List<object>();

		public MRPBomCache(PXGraph graph, bool fetchActiveBoms = true)
		{
			AMDebug.TraceWriteMethodName("Loading BOM Cache");
			this.graph = graph;
			if (fetchActiveBoms != true) return;
			PXCache cache = graph.Caches[typeof(AMBomItem)];
			var bomComparer = cache.GetComparer();
			boms = new Dictionary<AMBomItem, CacheItem>(bomComparer);
			bomRevisions = new Dictionary<string, List<AMBomItem>>();

			PXResultset<AMBomItem> result = PXSelectJoin<AMBomItem,
				InnerJoin<AMBomOper,
						On<AMBomOper.bOMID, Equal<AMBomItem.bOMID>,
						And<AMBomOper.revisionID, Equal<AMBomItem.revisionID>>>,
				InnerJoin<AMWC, On<AMWC.wcID, Equal<AMBomOper.wcID>>,
				LeftJoin<AMBomMatl,
					On<AMBomMatl.bOMID, Equal<AMBomOper.bOMID>,
					And<AMBomMatl.revisionID, Equal<AMBomOper.revisionID>,
					And<AMBomMatl.operationID, Equal<AMBomOper.operationID>>>>,
				LeftJoin<AMBomMatlCury,
					On<AMBomMatlCury.bOMID, Equal<AMBomMatl.bOMID>,
					And<AMBomMatlCury.revisionID, Equal<AMBomMatl.revisionID>,
					And<AMBomMatlCury.operationID, Equal<AMBomMatl.operationID>,
					And<AMBomMatlCury.lineID, Equal<AMBomMatl.lineID>>>>>>>>>,
				Where<AMBomItem.status, Equal<AMBomStatus.active>,
					And<Where<AMBomMatlCury.curyID, IsNull,
						Or<AMBomMatlCury.curyID, Equal<Required<AMBomMatlCury.curyID>>>>>>,
				OrderBy<
					Asc<AMBomItem.bOMID,
					Asc<AMBomItem.revisionID,
					Asc<AMBomOper.operationCD,
					Asc<AMBomMatl.sortOrder>>>>>>
				.Select(graph, graph.Accessinfo.BaseCuryID);

			AMBomItem prevItem = null;
			AMBomOper prevOper = null;
			CacheItem current = null;
			List<AMBomItem> revisions = null;
			foreach (PXResult<AMBomItem> record in result)
			{
				AMBomItem item = record;
				AMBomOper oper = PXResult.Unwrap<AMBomOper>(record);
				AMWC awc = PXResult.Unwrap<AMWC>(record);
				AMBomMatl matl = PXResult.Unwrap<AMBomMatl>(record);
				AMBomMatlCury matlCury = PXResult.Unwrap<AMBomMatlCury>(record);

				if (prevItem == null || !bomComparer.Equals(prevItem, item))
				{
					boms[item] = current = new CacheItem();
					if (prevItem == null || item.BOMID != prevItem.BOMID)
						bomRevisions[item.BOMID] = revisions = new List<AMBomItem>();
					prevOper = null;
					revisions.Add(item);
				}

				if (oper.OperationID != null && oper.OperationID != prevOper?.OperationID)
				{
					current.Operations.Add(new PXResult<AMBomOper, AMWC>(oper, awc));
					current.OperationPhantoms[oper.OperationID.Value] = new List<object>();
				}

				if (matl?.LineID != null)
				{
					AllMaterialCache.Add(new PXResult<AMBomMatl, AMBomItem, AMBomMatlCury>(matl, item, matlCury));
					if (oper.OperationID != null && matl.MaterialType == AMMaterialType.Phantom)
						current.OperationPhantoms[oper.OperationID.Value].Add(new PXResult<AMBomMatl>(matl));
				}

				prevOper = oper;
				prevItem = item;
			}
		}

		public AMBomItem GetActiveRevisionBomItemByDate(string bomId, DateTime date)
		{
			if (bomRevisions != null)
			{
				bomRevisions.TryGetValue(bomId, out var bomList);
				if (bomList != null)
				{
					var orderedList = bomList.Count == 1 ? bomList : bomList.OrderByDescending(o => o.EffStartDate).ThenByDescending(o => o.RevisionID).ToList();
					foreach (var bomItem in orderedList)
					{
						if (date.BetweenInclusive(bomItem.EffStartDate, (bomItem.EffEndDate ?? Common.Dates.EndOfTimeDate)))
						{
							return bomItem;
						}
					}
				}

				return null;
			}

			return PrimaryBomIDManager.GetActiveRevisionBomItemByDate(graph, bomId, date);
		}

		public AMBomItem GetActiveRevisionBomItem(string bomId)
		{
			if (string.IsNullOrWhiteSpace(bomId))
			{
				return null;
			}

			if (bomRevisions != null)
			{
				AMBomItem result = null;
				foreach (AMBomItem i in bomRevisions[bomId])
				{
					if (result == null || result.EffStartDate < i.EffStartDate)
						result = i;
				}

				return result;
			}
			return PrimaryBomIDManager.GetActiveRevisionBomItem(graph, bomId);
		}

		public void ActivateBomRevision(string bomId, string revisionId)
		{
			ActivateBomRevision(new AMBomItem() { BOMID = bomId, RevisionID = revisionId });
		}

		public void ActivateBomRevision(AMBomItem item)
		{
			if (boms != null)
			{
				if (boms.TryGetValue(item, out var detail))
				{
					ActivateBomRevision(item, detail);
					return;
				}
			}
			ReadBomRevision(item.BOMID, item.RevisionID);
		}

		protected virtual void ActivateBomRevision(AMBomItem item, CacheItem detail)
		{
			PXSelectJoin<AMBomOper,
				InnerJoin<AMWC, On<AMWC.wcID, Equal<AMBomOper.wcID>>>,
			Where<AMBomOper.bOMID, Equal<Required<AMBomOper.bOMID>>,
				And<AMBomOper.revisionID, Equal<Required<AMBomOper.revisionID>>>>,
			OrderBy<
				Asc<AMBomOper.operationCD>>>
			.StoreResult(graph, detail.Operations,
					PXQueryParameters.ExplicitParameters(item.BOMID, item.RevisionID), true);
			bool clearPhantoms = true;
			foreach (int operationID in detail.OperationPhantoms.Keys)
			{
				PXSelect<AMBomMatl,
				Where<AMBomMatl.bOMID, Equal<Required<AMBomOper.bOMID>>,
					And<AMBomMatl.revisionID, Equal<Required<AMBomOper.revisionID>>,
					And<AMBomMatl.operationID, Equal<Required<AMBomOper.operationID>>,
					And<AMBomMatl.materialType, Equal<AMMaterialType.phantom>>>>>,
				OrderBy<Asc<AMBomMatl.sortOrder, Asc<AMBomMatl.lineID>>>>
					.StoreResult(graph, detail.OperationPhantoms[operationID],
						PXQueryParameters.ExplicitParameters(item.BOMID, item.RevisionID, operationID), clearPhantoms);
				clearPhantoms = false;
			}
		}

		public void ReadBomRevision(string bomId, string revisionId)
		{
			if (bomId == null) return;
			PXResultset<AMBomItem> result = PXSelectJoin<AMBomItem,
			InnerJoin<AMBomOper,
					On<AMBomOper.bOMID, Equal<AMBomItem.bOMID>,
					And<AMBomOper.revisionID, Equal<AMBomItem.revisionID>>>,
			InnerJoin<AMWC, On<AMWC.wcID, Equal<AMBomOper.wcID>>,
			LeftJoin<AMBomMatl,
				On<AMBomMatl.bOMID, Equal<AMBomOper.bOMID>,
				And<AMBomMatl.revisionID, Equal<AMBomOper.revisionID>,
				And<AMBomMatl.operationID, Equal<AMBomOper.operationID>>>>>>>,
			Where<AMBomItem.bOMID, Equal<Required<AMBomOper.bOMID>>,
				And<AMBomItem.revisionID, Equal<Required<AMBomOper.revisionID>>>>,
			OrderBy<
				Asc<AMBomOper.operationCD,
				Asc<AMBomMatl.sortOrder,
				Asc<AMBomMatl.lineID>>>>>
			.Select(graph, bomId, revisionId);

			var operList = new List<object>();
			var operPhantomsList = new List<object>();
			var matlList = new List<object>();
			AMBomOper prevOper = null;
			bool clearPhantoms = true;

			foreach (PXResult<AMBomItem> record in result)
			{
				AMBomItem item = record;
				AMBomOper oper = PXResult.Unwrap<AMBomOper>(record);
				AMWC awc = PXResult.Unwrap<AMWC>(record);
				AMBomMatl matl = PXResult.Unwrap<AMBomMatl>(record);

				if (oper.OperationID != prevOper?.OperationID)
				{
					operList.Add(new PXResult<AMBomOper, AMWC>(oper, awc));
					if (prevOper != null)
					{
						PXSelect<AMBomMatl,
						Where<AMBomMatl.bOMID, Equal<Required<AMBomOper.bOMID>>,
							And<AMBomMatl.revisionID, Equal<Required<AMBomOper.revisionID>>,
							And<AMBomMatl.operationID, Equal<Required<AMBomOper.operationID>>,
							And<AMBomMatl.materialType, Equal<AMMaterialType.phantom>>>>>,
						OrderBy<Asc<AMBomMatl.sortOrder, Asc<AMBomMatl.lineID>>>>
						.StoreResult(graph, operPhantomsList,
							PXQueryParameters.ExplicitParameters(bomId, revisionId, prevOper.OperationID), clearPhantoms);
						operPhantomsList = new List<object>();
						clearPhantoms = false;
					}
				}
				if (matl.LineID != null)
				{
					matlList.Add(new PXResult<AMBomMatl, AMBomItem>(matl, item));
					if (matl.MaterialType == AMMaterialType.Phantom)
						operPhantomsList.Add(new PXResult<AMBomMatl>(matl));
				}
				prevOper = oper;
			}

			if (prevOper != null)
			{
				PXSelect<AMBomMatl,
				Where<AMBomMatl.bOMID, Equal<Required<AMBomOper.bOMID>>,
					And<AMBomMatl.revisionID, Equal<Required<AMBomOper.revisionID>>,
					And<AMBomMatl.operationID, Equal<Required<AMBomOper.operationID>>,
					And<AMBomMatl.materialType, Equal<AMMaterialType.phantom>>>>>,
				OrderBy<Asc<AMBomMatl.sortOrder, Asc<AMBomMatl.lineID>>>>
				.StoreResult(graph, operPhantomsList,
					PXQueryParameters.ExplicitParameters(bomId, revisionId, prevOper.OperationID), clearPhantoms);
			}
			PXSelectJoin<
				AMBomMatl,
				InnerJoin<AMBomItem,
					On<AMBomMatl.bOMID, Equal<AMBomItem.bOMID>,
						And<AMBomMatl.revisionID, Equal<AMBomItem.revisionID>>>>,
				Where<AMBomMatl.bOMID, Equal<Required<AMBomOper.bOMID>>,
					And<AMBomMatl.revisionID, Equal<Required<AMBomOper.revisionID>>>>>
			.StoreResult(graph, matlList,
				PXQueryParameters.ExplicitParameters(bomId, revisionId), true);

			PXSelectJoin<AMBomOper,
			InnerJoin<AMWC, On<AMWC.wcID, Equal<AMBomOper.wcID>>>,
			Where<AMBomOper.bOMID, Equal<Required<AMBomOper.bOMID>>,
				And<AMBomOper.revisionID, Equal<Required<AMBomOper.revisionID>>>>,
			OrderBy<
				Asc<AMBomOper.operationCD>>>
			.StoreResult(graph, operList,
				PXQueryParameters.ExplicitParameters(bomId, revisionId), true);
		}
	}
}
