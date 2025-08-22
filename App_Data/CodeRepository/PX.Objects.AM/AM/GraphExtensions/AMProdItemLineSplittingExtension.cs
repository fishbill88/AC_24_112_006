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
using PX.Data;
using PX.Objects.Common;
using PX.Objects.IN;
using LotSerOptions = PX.Objects.IN.LSSelect.LotSerOptions;
using Counters = PX.Objects.IN.LSSelect.Counters;
using PX.Common;

namespace PX.Objects.AM
{
	public abstract class AMProdItemLineSplittingExtension<TGraph> : IN.GraphExtensions.LineSplittingExtension<TGraph, AMProdItem, AMProdItem, AMProdItemSplit>
		where TGraph : PXGraph
	{
		#region Configuration
		protected override Type SplitsToDocumentCondition => typeof(AMProdItemSplit.FK.ProductionOrder.SameAsCurrent);

		protected override Type LineQtyField => typeof(AMProdItem.qtytoProd);

		public bool IsEmptyLotSerialDefaultScope = false;

		public override AMProdItemSplit LineToSplit(AMProdItem item)
		{
			using (new InvtMultScope(item))
			{
				AMProdItemSplit ret = (AMProdItemSplit)item;
				ret.BaseQty = item.BaseQty - item.UnassignedQty;
				return ret;
			}
		}
		#endregion

		#region Event Handlers
		#region AMProdItem
		protected override void SubscribeForLineEvents()
		{
			base.SubscribeForLineEvents();
			ManualEvent.FieldOf<AMProdItem, AMProdItem.qtytoProd>.Verifying.Subscribe<decimal?>(Base, EventHandler);
		}

		public virtual void EventHandler(ManualEvent.FieldOf<AMProdItem, AMProdItem.qtytoProd>.Verifying.Args<decimal?> e)
		{
			if (e.NewValue < 0m)
				throw new PXSetPropertyException(Messages.EntryGreaterEqualZero, PXErrorLevel.Error, 0);
		}

		protected override void EventHandler(ManualEvent.Row<AMProdItem>.Inserted.Args e)
		{
			if (e.Row.InvtMult != 0)
			{
				base.EventHandler(e);
				return;
			}

			if(e.Row?.PreassignLotSerial == true)
			{
				return;
			}

			e.Cache.SetValue<AMProdItem.lotSerialNbr>(e.Row, null);
			e.Cache.SetValue<AMProdItem.expireDate>(e.Row, null);
		}

		protected override void EventHandler(ManualEvent.Row<AMProdItem>.Updated.Args e)
		{
			var scopeBefore = IsEmptyLotSerialDefaultScope;
			IsEmptyLotSerialDefaultScope = scopeBefore || (e.Row?.InventoryID != null && !e.Cache.ObjectsEqual<AMProdItem.qtytoProd>(e.Row, e.OldRow) && IsPreassignedManualAssignRequired(e.Row));
			try
			{
			base.EventHandler(e);
			}
			finally
			{
				IsEmptyLotSerialDefaultScope = scopeBefore;
			}

			if(e.Row?.PreassignLotSerial == true)
			{
				return;
			}

			e.Cache.SetValue<AMProdItem.lotSerialNbr>(e.Row, null);
			e.Cache.SetValue<AMProdItem.expireDate>(e.Row, null);
		}

		protected override void EventHandler(ManualEvent.Row<AMProdItem>.Persisting.Args e)
		{
			var isPreassigned = e.Row?.PreassignLotSerial == true; 
			var isLotTracked = false;
			if(isPreassigned)
			{
				var result = (INLotSerClass)ReadInventoryItem(e.Row.InventoryID);
				isLotTracked = result != null && result.LotSerTrack == INLotSerTrack.LotNumbered;
				if(isLotTracked && !string.IsNullOrWhiteSpace(e.Row?.LotSerialNbr))
				{
					// Trigger reset of LotSerialNbr in base
					LineCounters[e.Row] = new Counters { UnassignedNumber = 1 };
				}
			}

			if (!isPreassigned)
			{
				//for normal orders there are only when received numbers which do not require any additional processing
				LineCounters[e.Row] = new Counters { UnassignedNumber = 0 };
			}

			base.EventHandler(e);

			if(isPreassigned && isLotTracked && string.IsNullOrWhiteSpace(e.Row.LotSerialNbr))
			{
				var splits = SelectSplits(e.Row);
				if (splits.Count() > 1)
				{
					e.Row.LotSerialNbr = null;
				}
				else
				{
					// User changes lot number used on order...
					var split = splits.FirstOrDefault();
					if(!string.IsNullOrWhiteSpace(split?.LotSerialNbr))
						e.Row.LotSerialNbr = split.LotSerialNbr;
				}
			}
		}
		#endregion
		#region AMProdItemSplit
		protected override void SubscribeForSplitEvents()
		{
			base.SubscribeForSplitEvents();
			ManualEvent.FieldOf<AMProdItemSplit, AMProdItemSplit.invtMult>.Defaulting.Subscribe<short?>(Base, EventHandler);
			ManualEvent.FieldOf<AMProdItemSplit, AMProdItemSplit.subItemID>.Defaulting.Subscribe<int?>(Base, EventHandler);
			ManualEvent.FieldOf<AMProdItemSplit, AMProdItemSplit.locationID>.Defaulting.Subscribe<int?>(Base, EventHandler);
			ManualEvent.FieldOf<AMProdItemSplit, AMProdItemSplit.lotSerialNbr>.Updated.Subscribe<string>(Base, EventHandler);
		}

		protected virtual void EventHandler(ManualEvent.FieldOf<AMProdItemSplit, AMProdItemSplit.invtMult>.Defaulting.Args<short?> e)
		{
			if (LineCurrent != null && (e.Row == null || e.Row.ProdOrdID == LineCurrent.ProdOrdID))
			{
				using (new InvtMultScope(LineCurrent))
				{
					e.NewValue = LineCurrent.InvtMult;
					e.Cancel = true;
				}
			}
		}

		protected virtual void EventHandler(ManualEvent.FieldOf<AMProdItemSplit, AMProdItemSplit.subItemID>.Defaulting.Args<int?> e)
		{
			if (LineCurrent != null && (e.Row == null || e.Row.ProdOrdID == LineCurrent.ProdOrdID))
			{
				e.NewValue = LineCurrent.SubItemID;
				e.Cancel = true;
			}
		}

		protected virtual void EventHandler(ManualEvent.FieldOf<AMProdItemSplit, AMProdItemSplit.locationID>.Defaulting.Args<int?> e)
		{
			if (LineCurrent != null && (e.Row == null || e.Row.ProdOrdID == LineCurrent.ProdOrdID))
			{
				e.NewValue = LineCurrent.LocationID;
				e.Cancel = true;
			}
		}

		protected virtual void EventHandler(ManualEvent.FieldOf<AMProdItemSplit, AMProdItemSplit.lotSerialNbr>.Updated.Args<string> e)
		{
			if(e.Row?.LotSerialNbr == null)
			{
				return;
			}

			var parent = (AMProdItem)PXParentAttribute.SelectParent(e.Cache, e.Row, typeof(AMProdItem));
			if(parent?.InventoryID == null)
			{
				return;
			}

			if (LineCache.GetStatus(parent) == PXEntryStatus.Inserted && !string.IsNullOrWhiteSpace(e.OldValue) && parent.LotSerialNbr == e.OldValue)
			{
				parent.LotSerialNbr = e.Row.LotSerialNbr;
			}

			if (IsPreassignedManualAssignRequired(parent) && !IsEmptyLotSerialDefaultScope && string.IsNullOrWhiteSpace(e.OldValue) && !string.IsNullOrWhiteSpace(e.Row.LotSerialNbr) && ReadInventoryItem(parent.InventoryID).GetItem<INLotSerClass>()?.LotSerTrack == INLotSerTrack.SerialNumbered)
			{
				var qtyChanged = e.Row.Qty.GetValueOrDefault() - 1;
				e.Cache.SetValueExt<AMProdItemSplit.qty>(e.Row, 1m);
				if (qtyChanged == 0)
			{
				return;
			}
				SyncEmptyLotSerialDefaultQty(parent, qtyChanged);
			}
		}

		protected override void EventHandler(ManualEvent.Row<AMProdItemSplit>.Deleted.Args e)
			{
			base.EventHandler(e);

			var parent = (AMProdItem)PXParentAttribute.SelectParent(e.Cache, e.Row, typeof(AMProdItem));
			if (parent == null || Base.Caches[typeof(AMProdItem)].GetStatus(parent).IsDeleted() || !IsPreassignedManualAssignRequired(parent))
			{
				return;
			}
			var qtyChanged = ((parent.BaseQty ?? 0m) - GetLotSerialAssignedQty(parent) - (GetEmptySplit(parent)?.Qty ?? 0)).NotLessZero();
			SyncEmptyLotSerialDefaultQty(parent, qtyChanged);
		}

		protected override void EventHandler(ManualEvent.Row<AMProdItemSplit>.Inserting.Args e)
		{
			if (e.Row != null && !e.ExternalCall && CurrentOperation == PXDBOperation.Update)
			{
				var splitsExist = false;
				foreach (var siblingSplit in SelectSplits(e.Row))
				{
					splitsExist = true;
					if (AreSplitsEqual(e.Row, siblingSplit))
					{
						var oldSiblingSplit = PXCache<AMProdItemSplit>.CreateCopy(siblingSplit);
#if DEBUG
						AMDebug.TraceWriteMethodName($"Changing insert to an update. Adding {e.Row.BaseQty} to {siblingSplit.BaseQty}");
#endif
#pragma warning disable PX1048
						siblingSplit.BaseQty += e.Row.BaseQty.GetValueOrDefault();
						siblingSplit.Qty = INUnitAttribute.ConvertFromBase(e.Cache, siblingSplit.InventoryID, siblingSplit.UOM, siblingSplit.BaseQty.GetValueOrDefault(), INPrecision.QUANTITY);
#pragma warning restore PX1048
						e.Cache.Current = siblingSplit;
						e.Cache.RaiseRowUpdated(siblingSplit, oldSiblingSplit);
						e.Cache.MarkUpdated(siblingSplit);
						e.Cancel = true;
						break;
					}
				}

				if (!splitsExist && e.Row.InventoryID != null && IsEmptyLotSerialDefaultScope)
				{
					var parent = (AMProdItem)PXParentAttribute.SelectParent(e.Cache, e.Row, typeof(AMProdItem));
					if (parent != null)
					{
						e.Row.Qty = parent.Qty;
					}
				}
			}

			if (e.Row != null && !e.Cancel && (e.Row.InventoryID == null || string.IsNullOrEmpty(e.Row.UOM)))
				e.Cancel = true;

			if (!e.Cancel)
				base.EventHandler(e);
		}

		public override void EventHandler(ManualEvent.Row<AMProdItemSplit>.Persisting.Args e)
		{
			base.EventHandler(e);

			if (e.Row == null || e.Row.Qty > 0)
				return;

			e.Cache.RaiseExceptionHandling<AMProdItemSplit.qty>(e.Row, null, new PXSetPropertyException(Messages.QuantityGreaterThanZero));
		}

		public override void EventHandlerQty(ManualEvent.FieldOf<AMProdItemSplit>.Verifying.Args<decimal?> e)
		{
			AMProdItemSplit split = e.Row;
			AMProdItem parent = LineCurrent;

			if (parent != null && parent.PreassignLotSerial == true)
			{
				if (!IsEmptyLotSerialDefaultScope)
				{
					var result = (INLotSerClass)base.ReadInventoryItem(e.Row?.InventoryID);
					var newValueDecimal = (e.NewValue != null) && (e.NewValue is decimal) ? (decimal)e.NewValue : 0m;
					if (result != null && result.LotSerTrack == INLotSerTrack.SerialNumbered && result.LotSerAssign == INLotSerAssign.WhenReceived && newValueDecimal != 0M && newValueDecimal != 1M)
				{
					e.NewValue = 1M;
				}
			}
				return;
			}

				e.NewValue = VerifySNQuantity(e.Cache, e.Row, e.NewValue, nameof(AMProdItemSplit.qty));
		}
		#endregion
		#region LotSerOptions
		protected override void EventHandler(ManualEvent.Row<LotSerOptions>.Selected.Args e)
		{
			AMProdItem item = LineCurrent;
			bool enableGenerate = (item != null && item.PreassignLotSerial == true);

			PXUIFieldAttribute.SetEnabled<LotSerOptions.startNumVal>(e.Cache, e.Row, enableGenerate);
			PXUIFieldAttribute.SetEnabled<LotSerOptions.qty>(e.Cache, e.Row, enableGenerate);
			PXDBDecimalAttribute.SetPrecision(e.Cache, e.Row, nameof(LotSerOptions.Qty), e.Row.IsSerial == true ? 0 : CommonSetupDecPl.Qty);
			generateNumbers.SetEnabled(enableGenerate);
		}
		#endregion
		#endregion

		#region UpdateParent Helper
		public override void UpdateParent(AMProdItem line)
		{
			if (line != null)
			{
				UpdateParent(line, null, null, out _);
			}
			else
			{
				base.UpdateParent(line);
			}
		}

		public override AMProdItem UpdateParent(AMProdItemSplit newSplit, AMProdItemSplit oldSplit)
		{
			AMProdItemSplit anySplit = newSplit ?? oldSplit;
			AMProdItem parent = (AMProdItem)LSParentAttribute.SelectParent(SplitCache, newSplit ?? oldSplit, typeof(AMProdItem));

			if (parent != null)
			{
				if (anySplit != null && SameInventoryItem(anySplit, parent))
				{
					var oldParent = PXCache<AMProdItem>.CreateCopy(parent);
					using (new InvtMultScope(parent))
					{
						parent.UnassignedQty = 0m;
						if (newSplit != null)
						{
							if (IsLotSerialItem(newSplit)){
							var hasLotSerialNbrQty = SelectSplits(newSplit).Where(s => s.LotSerialNbr != null).Sum(s => s.BaseQty) ?? 0m;
							parent.UnassignedQty = (parent.BaseQtytoProd - hasLotSerialNbrQty).NotLessZero();
						}
						}
						else
						{
							return base.UpdateParent(newSplit, oldSplit);
						}
					}

					LineCache.MarkUpdated(parent);
					LineCache.RaiseFieldUpdated(LineQtyField.Name, parent, oldParent.Qty);

					if (LineCache.RaiseRowUpdating(oldParent, parent))
						LineCache.RaiseRowUpdated(parent, oldParent);
					else
						PXCache<AMProdItem>.RestoreCopy(parent, oldParent);
				}
				return parent;
			}

			return base.UpdateParent(newSplit, oldSplit);
		}

		public override void UpdateParent(AMProdItem line, AMProdItemSplit newSplit, AMProdItemSplit oldSplit, out decimal baseQty)
		{
			ResetAvailabilityCounters(line);

			bool counted = LineCounters.ContainsKey(line);

			base.UpdateParent(line, newSplit, oldSplit, out baseQty);

			if (!counted && oldSplit != null && LineCounters.TryGetValue(line, out Counters counters))
				baseQty = counters.BaseQty;
		}

		public static void ResetAvailabilityCounters(AMProdItem line)
		{
			line.LineQtyAvail = null;
			line.LineQtyHardAvail = null;
		}
		#endregion

		#region Create/Truncate/Update/Issue Numbers
		public override void UpdateNumbers(AMProdItem line)
		{
			if (line != null)
				LineCounters.Remove(line);

			foreach (AMProdItemSplit split in SelectSplits(line))
			{
				AMProdItemSplit newSplit = PXCache<AMProdItemSplit>.CreateCopy(split);

				if (line.LocationID == null && newSplit.LocationID != null && SplitCache.GetStatus(newSplit) == PXEntryStatus.Inserted && newSplit.Qty == 0m)
				{
					SplitCache.Delete(newSplit);
				}
				else
				{
					newSplit.SubItemID = line.SubItemID ?? newSplit.SubItemID;
					newSplit.SiteID = line.SiteID;
					newSplit.ExpireDate = ExpireDateByLot(newSplit, line);
					newSplit.LocationID = line.LocationID;
					SplitCache.Update(newSplit);
				}
			}
		}

		protected virtual bool IsPreassignedManualAssignRequired(AMProdItem prodItem)
		{
			if (prodItem?.PreassignLotSerial != true)
			{
				return false;
			}

			var lsClass = (INLotSerClass)ReadInventoryItem(prodItem.InventoryID);

			return lsClass != null
				&& lsClass.LotSerTrack != INLotSerTrack.NotNumbered
				&& lsClass.LotSerAssign == INLotSerAssign.WhenReceived
				&& lsClass.AutoNextNbr == false;
		}

		public override void CreateNumbers(AMProdItem line, decimal deltaBaseQty, bool forceAutoNextNbr)
		{
			if (TryCreateManualAssignLineDetail(line, deltaBaseQty) || IsEmptyLotSerialDefaultScope)
		{
				return;
			}

			base.CreateNumbers(line, deltaBaseQty, forceAutoNextNbr);
		}

		protected virtual bool TryCreateManualAssignLineDetail(AMProdItem line, decimal deltaBaseQty)
		{
			if (deltaBaseQty < 0 || !IsPreassignedManualAssignRequired(line))
			{
				return false;
			}

			var newSplit = CreateNoLotSerialLineDetail(line, deltaBaseQty);
			if (newSplit?.Qty == null)
			{
				return false;
			}

			var splitQty = newSplit.BaseQty.GetValueOrDefault();
			if (splitQty > 0m && decimal.Remainder(splitQty, 1m) == 0m)
			{
				line.UnassignedQty = (line.UnassignedQty - splitQty).NotLessZero();
			}

			if (line.UnassignedQty > 0)
			{
				RaiseUnassignedExceptionHandling(line);
			}

			return true;
		}

		protected virtual AMProdItemSplit CreateNoLotSerialLineDetail(AMProdItem line, decimal baseQty)
		{
			var valueBefore = IsEmptyLotSerialDefaultScope;
			try
			{
				IsEmptyLotSerialDefaultScope = true;
				return (AMProdItemSplit)SplitCache.Insert(new AMProdItemSplit
				{
					AssignedNbr = string.Empty,
					LotSerialNbr = string.Empty,
					LotSerClassID = string.Empty,
					BaseQty = baseQty,
					Qty = baseQty
				});
			}
			finally
			{
				IsEmptyLotSerialDefaultScope = valueBefore;
			}
		}

		protected virtual AMProdItemSplit SyncEmptyLotSerialDefaultQty(AMProdItem prodItem, decimal qtyChange)
		{
			if (prodItem?.InventoryID == null || qtyChange == 0)
			{
				return null;
			}

			var emptySplit = GetEmptySplit(prodItem);
			if (emptySplit?.InventoryID != null)
			{
				return UpdateEmptyLotSerialDefaultQty(emptySplit, emptySplit.Qty.GetValueOrDefault() + qtyChange);
			}

			if (qtyChange > 0)
			{
				return CreateNoLotSerialLineDetail(prodItem, qtyChange);
			}
			return null;
		}

		protected virtual AMProdItemSplit GetEmptySplit(AMProdItem prodItem) => SelectSplits(prodItem)?.Where(r => IsEmptyLotSerialSplit(r))?.FirstOrDefault();

		protected virtual decimal GetLotSerialAssignedQty(AMProdItem prodItem) => SelectSplits(prodItem)?.Where(r => !IsEmptyLotSerialSplit(r))?.Sum(f => f.BaseQty) ?? 0m;

		protected virtual AMProdItemSplit UpdateEmptyLotSerialDefaultQty(AMProdItemSplit row, decimal qty)
		{
			if (row == null)
			{
				return null;
			}

			var valueBefore = IsEmptyLotSerialDefaultScope;
			try
			{
				IsEmptyLotSerialDefaultScope = true;
				row.Qty = qty;
				return row.Qty <= 0 ? (AMProdItemSplit)SplitCache.Delete(row) : (AMProdItemSplit)SplitCache.Update(row);
			}
			finally
			{
				IsEmptyLotSerialDefaultScope = valueBefore;
		}
		}

		protected virtual bool IsEmptyLotSerialSplit(AMProdItemSplit row) => string.IsNullOrWhiteSpace(row?.LotSerialNbr);

		#endregion

		public AMProdItemSplit[] GetSplits(AMProdItem line) => SelectSplits(line);

		public INLotSerClass GetLotSerClass(ILSMaster line) => ReadInventoryItem(line.InventoryID);

		protected override INLotSerTrack.Mode GetTranTrackMode(ILSMaster row, INLotSerClass lotSerClass)
		{
			if (row is AMProdItem && ((AMProdItem)row).PreassignLotSerial != true)
				return INLotSerTrack.Mode.None;

			return base.GetTranTrackMode(row, lotSerClass);
		}

		public virtual bool IsLotSerialItem(ILSMaster line)
		{
			PXResult<InventoryItem, INLotSerClass> item = ReadInventoryItem(line.InventoryID);

			if (item == null)
				return false;

			return INLotSerialNbrAttribute.IsTrack(item, line.TranType, line.InvtMult);
		}
	}
}
