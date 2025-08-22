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
using PX.Objects.CS;
using System;

namespace PX.Objects.PM
{
	public abstract class PMCommitmentAttribute : PXEventSubscriberAttribute, IPXRowInsertedSubscriber, IPXRowUpdatedSubscriber, IPXRowDeletedSubscriber
	{
		protected Type primaryEntity;
		protected Type detailEntity;

		public PMCommitmentAttribute(Type primaryEntity)
		{
			this.primaryEntity = primaryEntity;
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			this.detailEntity = sender.GetItemType();

			if (!PXAccess.FeatureInstalled<FeaturesSet.projectModule>())
				return;

			sender.Graph.RowUpdated.AddHandler(primaryEntity, DocumentRowUpdated);
		}

		public void RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			Guid? commitmentID = (Guid?)sender.GetValue(e.Row, FieldOrdinal);

			DeleteCommitment(sender, commitmentID);
		}

		public void RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			SyncCommitment(sender, e.Row);
		}

		public void RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if (IsCommitmentSyncRequired(sender, e.Row, e.OldRow))
			{
				SyncCommitment(sender, e.Row);
			}
		}

		public abstract void DocumentRowUpdated(PXCache sender, PXRowUpdatedEventArgs e);

		protected abstract bool IsCommitmentSyncRequired(PXCache sender, object row, object oldRow);

		protected abstract bool EraseCommitment(PXCache sender, object row);

		protected abstract int? GetAccountGroup(PXCache sender, object row);

		protected abstract PMCommitment FromRecord(PXCache sender, object row);

		protected virtual void SyncCommitment(PXCache sender, object row)
		{
			SyncCommitment(sender, row, false);
		}
		protected virtual void SyncCommitment(PXCache sender, object row, bool skipCommitmentSelect)
		{
			if (!IsCommitmentTrackingEnabled(sender))
				return;

			PXCache detailCache = sender.Graph.Caches[detailEntity];
			Guid? commitmentID = (Guid?)detailCache.GetValue(row, FieldOrdinal);
			if (EraseCommitment(sender, row))
			{
				DeleteCommitment(sender, commitmentID);
				detailCache.SetValue(row, FieldOrdinal, null);
				detailCache.MarkUpdated(row, assertError: true);
			}
			else
			{
				int? accountGroup = GetAccountGroup(sender, row);

				PMCommitment commitment = null;
				if (!skipCommitmentSelect && commitmentID != null)
				{
					commitment = PXSelect<PMCommitment, Where<PMCommitment.commitmentID, Equal<Required<PMCommitment.commitmentID>>>>.Select(sender.Graph, commitmentID);
				}

				if (commitment == null)
				{
					commitment = FromRecord(sender, row);

					sender.Graph.Caches[typeof(PMCommitment)].Insert(commitment);

					if (commitment.CommitmentID != commitmentID)
					{
						detailCache.SetValue(row, FieldOrdinal, commitment.CommitmentID);
						detailCache.MarkUpdated(row, assertError: true);
					}
				}
				else
				{
					PMCommitment container = FromRecord(sender, row);
					commitment.AccountGroupID = accountGroup;
					commitment.Status = container.Status;
					commitment.ProjectID = container.ProjectID;
					commitment.ProjectTaskID = container.TaskID;
					commitment.UOM = container.UOM;
					commitment.OrigQty = container.OrigQty;
					commitment.OrigAmount = container.OrigAmount;
					commitment.Qty = container.Qty;
					commitment.Amount = container.Amount;
					commitment.ReceivedQty = container.ReceivedQty;
					commitment.OpenQty = container.OpenQty;
					commitment.OpenAmount = container.OpenAmount;
					commitment.InvoicedQty = container.InvoicedQty;
					commitment.InvoicedAmount = container.InvoicedAmount;
					commitment.RefNoteID = container.RefNoteID;
					commitment.InventoryID = container.InventoryID;
					commitment.CostCodeID = container.CostCodeID;
					commitment.BranchID = container.BranchID;

					sender.Graph.Caches[typeof(PMCommitment)].Update(commitment);

				}
			}
		}

		protected virtual void DeleteCommitment(PXCache sender, Guid? commitmentID)
		{
			if (commitmentID != null)
			{
				PMCommitment commitment = PXSelect<PMCommitment, Where<PMCommitment.commitmentID, Equal<Required<PMCommitment.commitmentID>>>>.Select(sender.Graph, commitmentID);
				if (commitment != null)
				{
					sender.Graph.Caches[typeof(PMCommitment)].Delete(commitment);
				}
			}
		}

		protected virtual bool IsCommitmentTrackingEnabled(PXCache sender)
		{
			if (!PXAccess.FeatureInstalled<FeaturesSet.projectModule>())
				return false;

			PMSetup setup = PXSelect<PMSetup>.Select(sender.Graph);

			if (setup == null)
				return false;

			return setup.CostCommitmentTracking == true;
		}

		public static decimal CuryConvCury(CM.Extensions.IPXCurrencyRate foundRate, decimal baseval, int? precision)
		{
			if (baseval == 0) return 0m;

			if (foundRate == null)
				throw new ArgumentNullException(nameof(foundRate));

			decimal rate;
			decimal curyval;
			try
			{
				rate = (decimal)foundRate.CuryRate;
			}
			catch (InvalidOperationException)
			{
				throw new CM.PXRateNotFoundException();
			}
			if (rate == 0.0m)
			{
				rate = 1.0m;
			}
			bool mult = foundRate.CuryMultDiv != "D";
			curyval = mult ? (decimal)baseval * rate : (decimal)baseval / rate;

			if (precision.HasValue)
			{
				curyval = Decimal.Round(curyval, precision.Value, MidpointRounding.AwayFromZero);
			}

			return curyval;
		}

		public static void Sync(PXCache sender, object data)
		{
			PMCommitmentAttribute commitmentAttribute = null;
			foreach (PXEventSubscriberAttribute attr in sender.GetAttributesReadonly("commitmentID"))
			{
				if (attr is PMCommitmentAttribute)
				{
					commitmentAttribute = (PMCommitmentAttribute)attr;
					break;
				}
			}

			if (commitmentAttribute != null)
			{
				commitmentAttribute.SyncCommitment(sender, data, true);
			}
		}
	}
}
