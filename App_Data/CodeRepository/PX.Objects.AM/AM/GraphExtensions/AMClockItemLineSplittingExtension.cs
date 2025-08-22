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
using PX.Objects.Common;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.AM.Attributes;
using System.Collections.Generic;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;

namespace PX.Objects.AM
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class AMClockItemLineSplittingExtension : IN.GraphExtensions.LineSplittingExtension<ClockEntry, AMClockItem, AMClockItem, AMClockItemSplit>
	{
		#region Configuration
		protected override Type SplitsToDocumentCondition => typeof(
			AMClockItemSplit.employeeID.IsEqual<AMClockItem.employeeID.FromCurrent>.
			And<AMClockItemSplit.lineNbr.IsEqual<int0>>);

		protected override Type LineQtyField => typeof(AMClockItem.qty);

		public override AMClockItemSplit LineToSplit(AMClockItem line)
		{
			using (new InvtMultScope(line))
			{
				AMClockItemSplit ret = line;
				ret.BaseQty = line.BaseQty - line.UnassignedQty;
				return ret;
			}
		}
		#endregion

		#region Event Handlers
		#region AMClockItem
		protected override void SubscribeForLineEvents()
		{
			base.SubscribeForLineEvents();
			ManualEvent.FieldOf<AMClockItem, AMClockItem.lastOper>.Updated.Subscribe<bool?>(Base, EventHandler);
			ManualEvent.FieldOf<AMClockItem, AMClockItem.qty>.Updated.Subscribe<decimal?>(Base, EventHandler);
			ManualEvent.FieldOf<AMClockItem, AMClockItem.operationID>.Updated.Subscribe<int?>(Base, EventHandler);
		}

		protected virtual void EventHandler(ManualEvent.FieldOf<AMClockItem, AMClockItem.lastOper>.Updated.Args<bool?> e)
		{
			SetTranTypeInvtMult(e.Row);
		}

		protected virtual void EventHandler(ManualEvent.FieldOf<AMClockItem, AMClockItem.qty>.Updated.Args<decimal?> e)
		{
			SetTranTypeInvtMult(e.Row);
		}

		protected virtual void EventHandler(ManualEvent.FieldOf<AMClockItem, AMClockItem.operationID>.Updated.Args<int?> e)
		{
			if (!string.IsNullOrWhiteSpace(e.Row?.ProdOrdID) && e.Row.OperationID != null)
				SetTranTypeInvtMult(e.Row);
		}

		protected override void EventHandler(ManualEvent.Row<AMClockItem>.Selected.Args e)
		{
			if (e.Row == null || string.IsNullOrWhiteSpace(e.Row.ProdOrdID))
				return;

			AllowSplits(e.Row.Qty > 0 && e.Row.IsClockedIn == true && e.Row.LastOper.GetValueOrDefault());
			if (e.Row.Qty > 0 && e.Row.IsClockedIn == true)
				e.Cache.RaiseFieldUpdated(LineQtyField.Name, e.Row, e.Row.Qty);
		}

		protected override void EventHandler(ManualEvent.Row<AMClockItem>.Inserted.Args e)
		{
			if (e.Row.InvtMult != 0)
			{
				base.EventHandler(e);
			}
			else
			{
				e.Cache.SetValue<AMClockItem.lotSerialNbr>(e.Row, null);
				e.Cache.SetValue<AMClockItem.expireDate>(e.Row, null);
			}
		}

		protected override void EventHandler(ManualEvent.Row<AMClockItem>.Updated.Args e)
		{
			if (e.Row?.InventoryID == null || e.Row.OperationID == null || string.IsNullOrWhiteSpace(e.Row.ProdOrdID))
				return;

			var amProdItem = (AMProdItem)PXSelectorAttribute.Select<AMClockItem.prodOrdID>(e.Cache, e.Row);
			if (amProdItem == null)
				return;

			if (e.OldRow.InventoryID != null && e.Row.InventoryID == null || e.Row.InventoryID != e.OldRow.InventoryID)
				foreach (AMClockItemSplit split in PXParentAttribute.SelectSiblings(SplitCache, (AMClockItemSplit)e.Row, typeof(AMClockItem)))
					SplitCache.Delete(split); //Change of item will need a change of splits

			if (e.Row.InvtMult != 0)
			{
				if (e.Row.TranType != e.OldRow.TranType)
					SyncSplitTranType(e.Row);

				if (amProdItem.LastOperationID.GetValueOrDefault() == e.Row.OperationID)
					base.EventHandler(e);

				return;
			}

			e.Cache.SetValue<AMClockItem.lotSerialNbr>(e.Row, null);
			e.Cache.SetValue<AMClockItem.expireDate>(e.Row, null);

			if (e.Row.InvtMult == 0)
			{
				var oldInvtMul = ((AMClockItem)e.OldRow)?.InvtMult ?? 0;
				if (oldInvtMul != 0)
				{
					foreach (var detail in SelectSplits(e.Row))
					{
						SplitCache.Delete(detail);
					}
				}
			}
		}

		protected override void EventHandler(ManualEvent.Row<AMClockItem>.Deleted.Args e)
		{
			if (e.Row.InvtMult != 0)
				base.EventHandler(e);
		}

		protected override void EventHandler(ManualEvent.Row<AMClockItem>.Persisting.Args e)
		{
			if (e.Operation.Command().IsIn(PXDBOperation.Insert, PXDBOperation.Update))
				if (Math.Abs(e.Row.BaseQty.Value) >= 0.0000005m && (e.Row.UnassignedQty >= 0.0000005m || e.Row.UnassignedQty <= -0.0000005m))
					if (e.Cache.RaiseExceptionHandling<AMClockItem.qty>(e.Row, e.Row.Qty, new PXSetPropertyException(IN.Messages.BinLotSerialNotAssigned)))
						throw new PXRowPersistingException(typeof(AMClockItem.qty).Name, e.Row.Qty, IN.Messages.BinLotSerialNotAssigned);

			base.EventHandler(e);
		}

		#endregion
		#region AMClockItemSplit
		protected override void SubscribeForSplitEvents()
		{
			base.SubscribeForSplitEvents();
			ManualEvent.FieldOf<AMClockItemSplit, AMClockItemSplit.invtMult>.Updated.Subscribe<short?>(Base, EventHandler);
			ManualEvent.FieldOf<AMClockItemSplit, AMClockItemSplit.invtMult>.Defaulting.Subscribe<short?>(Base, EventHandler);
			ManualEvent.FieldOf<AMClockItemSplit, AMClockItemSplit.subItemID>.Defaulting.Subscribe<int?>(Base, EventHandler);
			ManualEvent.FieldOf<AMClockItemSplit, AMClockItemSplit.locationID>.Defaulting.Subscribe<int?>(Base, EventHandler);
			ManualEvent.FieldOf<AMClockItemSplit, AMClockItemSplit.lotSerialNbr>.Defaulting.Subscribe<string>(Base, EventHandler);
		}

		protected virtual void EventHandler(ManualEvent.FieldOf<AMClockItemSplit, AMClockItemSplit.invtMult>.Updated.Args<short?> e)
		{
			if (LineCurrent != null && e.Row?.LineNbr == 0)
				e.Row.TranType = e.Row.InvtMult < 1 ? AMTranType.Adjustment : AMTranType.Receipt;
		}

		protected virtual void EventHandler(ManualEvent.FieldOf<AMClockItemSplit, AMClockItemSplit.invtMult>.Defaulting.Args<short?> e)
		{
			if (LineCurrent == null || e.Row == null || e.Row.LineNbr != 0)
				return;

#if DEBUG
			AMDebug.TraceWriteMethodName($"TranType = {e.Row.TranType} [{LineCurrent.TranType}]; InvtMult = {e.Row.InvtMult} [{LineCurrent.InvtMult}]; [{LineCurrent.DebuggerDisplay}]");
#endif
			//Not sure why we would ever want ot use InvtMultScope since it is changing the InvtMult value incorrectly on us when qty < 0
			using (new InvtMultScope(LineCurrent))
			{
				e.NewValue = LineCurrent.InvtMult;
				e.Cancel = true;
			}
		}

		protected virtual void EventHandler(ManualEvent.FieldOf<AMClockItemSplit, AMClockItemSplit.subItemID>.Defaulting.Args<int?> e)
		{
			if (LineCurrent != null && (e.Row == null || e.Row.LineNbr == 0))
			{
				e.NewValue = LineCurrent.SubItemID;
				e.Cancel = true;
			}
		}

		protected virtual void EventHandler(ManualEvent.FieldOf<AMClockItemSplit, AMClockItemSplit.locationID>.Defaulting.Args<int?> e)
		{
			if (LineCurrent != null && (e.Row == null || e.Row.LineNbr == 0))
			{
				e.NewValue = LineCurrent.LocationID;
				e.Cancel = true;
			}
		}

		protected virtual void EventHandler(ManualEvent.FieldOf<AMClockItemSplit, AMClockItemSplit.lotSerialNbr>.Defaulting.Args<string> e)
		{
			if (e.Row?.InventoryID == null)
				return;

			PXResult<InventoryItem, INLotSerClass> item = ReadInventoryItem(e.Row.InventoryID);

			if (e.Row.InvtMult == null)
				e.Cache.RaiseFieldDefaulting<AMClockItemSplit.invtMult>(e.Row, out _);

			if (e.Row.TranType == null)
				e.Cache.RaiseFieldDefaulting<AMClockItemSplit.tranType>(e.Row, out _);

			INLotSerTrack.Mode mode = GetTranTrackMode(e.Row, item);
			if (mode == INLotSerTrack.Mode.None || (mode & INLotSerTrack.Mode.Create) > 0)
			{
				foreach (AMClockItemSplit lssplit in INLotSerialNbrAttribute.CreateNumbers<AMClockItemSplit>(e.Cache, item, mode, 1m))
				{
					e.NewValue = lssplit.LotSerialNbr;
					e.Cancel = true;
				}
			}
			//otherwise default via attribute
		}

		public override void EventHandlerQty(ManualEvent.FieldOf<AMClockItemSplit>.Verifying.Args<decimal?> e)
		{
			base.EventHandlerQty(e);
			if (e.Row?.InventoryID != null)
			{
				(var _, var lsClass) = ReadInventoryItem(e.Row.InventoryID);
				if (lsClass.LotSerTrack == INLotSerTrack.SerialNumbered && lsClass.LotSerAssign == INLotSerAssign.WhenReceived)
					if (e.NewValue.IsNotIn(null, 0m, 1m))
						e.NewValue = 1M;
			}
		}

		protected override void EventHandler(ManualEvent.Row<AMClockItemSplit>.Inserting.Args e)
		{
			base.EventHandler(e);

			if (e.Row == null)
				return;

			var rowParent = PXParentAttribute.SelectParent<AMClockItem>(e.Cache, e.Row);
			if (rowParent == null)
				return;

			e.Row.TranType = rowParent.TranType ?? e.Row.TranType;
			e.Row.InvtMult = AMTranType.InvtMult(e.Row.TranType, rowParent.Qty);
		}
		#endregion
		#endregion

		protected virtual void AllowSplits(bool allow)
		{
			SplitCache.AllowInsert = allow && LineCache.AllowInsert;
			SplitCache.AllowUpdate = allow && LineCache.AllowUpdate;
		}

		protected virtual void SyncSplitTranType(AMClockItem line)
		{
			foreach (var split in PXParentAttribute
				.SelectSiblings(SplitCache, (AMClockItemSplit)line, typeof(AMClockItem))
				.Cast<AMClockItemSplit>()
				.Where(s => s.TranType != line.TranType))
			{
				var copy = PXCache<AMClockItemSplit>.CreateCopy(split);
				split.TranType = line.TranType;
				SplitCache.MarkUpdated(split);
				SplitCache.RaiseRowUpdated(split, copy);
			}
		}

		protected virtual void SetTranTypeInvtMult(AMClockItem line)
		{
			if (line == null)
				return;
#if DEBUG
			var tranTypeOld = line.TranType;
			var invtMultOld = line.InvtMult;
#endif
			var tranTypeNew = line.Qty.GetValueOrDefault() < 0 ?
				AMTranType.Adjustment : AMTranType.Receipt;
			var invtMultNew = line.LastOper.GetValueOrDefault() && line.Qty != 0m
				? AMTranType.InvtMult(tranTypeNew, line.Qty)
				: 0;

#if DEBUG
			AMDebug.TraceWriteMethodName($"TranType = {tranTypeNew} (old value = {tranTypeOld}); InvtMult = {invtMultNew} (old value = {invtMultOld})");
#endif
			var syncSplits = false;
			if (invtMultNew != line.InvtMult)
			{
				syncSplits |= line.InvtMult != null && invtMultNew != 0;
				LineCache.SetValueExt<AMClockItem.invtMult>(line, invtMultNew);
			}

			if (tranTypeNew != line.TranType)
			{
				syncSplits |= line.TranType != null;
				LineCache.SetValueExt<AMClockItem.tranType>(line, tranTypeNew);
			}

			if (syncSplits)
			{
				SyncSplitTranType(line);
			}
		}

		public override void CreateNumbers(AMClockItem Row, decimal BaseQty, bool forceAutoNextNbr)
		{
			if (Row?.InventoryID == null) return;
			AMProdItem prodItem = AMProdItem.PK.Find(Base, Row.OrderType, Row.ProdOrdID);

			(var _, var lsClass) = ReadInventoryItem(Row.InventoryID);
			var isPreassignedLotSerialDefaulting = prodItem.PreassignLotSerial == true && lsClass.LotSerTrack != INLotSerTrack.NotNumbered && lsClass.LotSerAssign != INLotSerAssign.WhenUsed;
			if(isPreassignedLotSerialDefaulting && lsClass.LotSerIssueMethod.Equals(INLotSerIssueMethod.UserEnterable) && string.IsNullOrEmpty(lsClass.LotSerFormatStr))
			{
				lsClass.LotSerFormatStr = "00000000";
			}
			base.CreateNumbers(Row, BaseQty, forceAutoNextNbr || isPreassignedLotSerialDefaulting);
			if (isPreassignedLotSerialDefaulting)
			{
				ProdItemSplitHelper.BuildPreassignNumbers(Base, Row, (AMClockItemSplit)Row);
			}
		}

		public override void TruncateNumbers(AMClockItem Row, decimal BaseQty)
		{
			base.TruncateNumbers(Row, BaseQty);
			AMProdItem prodItem = AMProdItem.PK.Find(Base, Row.OrderType, Row.ProdOrdID);
			if (prodItem.PreassignLotSerial == true)
			{
				ProdItemSplitHelper.BuildPreassignNumbers(Base, Row, (AMClockItemSplit)Row);
			}
		}
	}
}
