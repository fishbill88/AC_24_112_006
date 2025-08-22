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

namespace PX.Objects.PR
{
	public class SetBatchStatusAttribute : PXEventSubscriberAttribute, IPXRowUpdatingSubscriber, IPXRowInsertingSubscriber
	{
		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			sender.Graph.FieldUpdating.AddHandler(sender.GetItemType(), nameof(PRBatch.hold),
				(cache, e) =>
				{
					PXBoolAttribute.ConvertValue(e);

					var payBatch = e.Row as PRBatch;
					if (payBatch != null)
						StatusSet(cache, payBatch);
				});

			sender.Graph.FieldVerifying.AddHandler(sender.GetItemType(), nameof(PRBatch.status),
				(cache, e) => 
				{
					e.NewValue = cache.GetValue<PRBatch.status>(e.Row);
				});

			sender.Graph.RowSelected.AddHandler(sender.GetItemType(),
				(cache, e) =>
				{
					// See AC-182326 for reference.
					// The "StatusSet" method should not be called from the RowSelected event handler of a Generic Inquiry graph since
					// we just need to read the value of the 'Status' field from the database and not dynamically calculate it.
					// If the "StatusSet" method is called from the RowSelected handler of a Generic Inquiry,
					// the value of the "PRBatch.Status" field will be determined incorrectly if relevant flag fields ('Closed', 'Open', 'Hold')
					// are not included to the list of Generic Inquiry columns. In that case, the value of flags will be null,
					// making PRBatch 'Balanced' in a Generic Inquiry regardless of real value of the Status field in the database.
					if (cache.Graph is PXGenericInqGrph)
					{
						return;
					}

					var payBatch = e.Row as PRBatch;

					if (payBatch != null)
						StatusSet(cache, payBatch);
				});
		}

		protected virtual void StatusSet(PXCache cache, PRBatch payBatch)
		{
			if (payBatch.Closed == true)
			{
				payBatch.Status = BatchStatus.Closed;
				return;
			}
			if (payBatch.Open == true)
			{
				payBatch.Status = BatchStatus.Open;
				return;
			}
			if (payBatch.Hold == true)
			{
				payBatch.Status = BatchStatus.Hold;
				return;
			}
			payBatch.Status = BatchStatus.Balanced;
		}

		public virtual void RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			StatusSet(sender, (PRBatch)e.Row);
		}

		public virtual void RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			StatusSet(sender, (PRBatch)e.NewRow);
		}
	}
}
